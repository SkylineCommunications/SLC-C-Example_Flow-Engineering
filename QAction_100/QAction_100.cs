using System;
using System.Linq;

using Newtonsoft.Json;

using QAction_100;

using Skyline.DataMiner.Core.DataMinerSystem.Common;
using Skyline.DataMiner.Core.DataMinerSystem.Protocol;
using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
using Skyline.DataMiner.Net.Messages.SLDataGateway;
using Skyline.DataMiner.Scripting;

using Parameter = Skyline.DataMiner.Scripting.Parameter;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public class QAction
{
	private ControlSurfaceAction _nextAction = ControlSurfaceAction.Connect;

	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public void Run(SLProtocolExt protocol)
	{
		try
		{
			GetVirtualSignalGroups(protocol, out var srcVsg, out var dstVsg);

			switch (_nextAction)
			{
				case ControlSurfaceAction.Connect:
					PerformConnect(protocol, srcVsg, dstVsg);
					_nextAction = ControlSurfaceAction.Disconnect;

					break;

				case ControlSurfaceAction.Disconnect:
				default:
					PerformDisconnect(protocol, dstVsg);
					_nextAction = ControlSurfaceAction.Connect;

					break;
			}
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}

	private void PerformConnect(SLProtocolExt protocol, DomInstance source, DomInstance destination)
	{
		protocol.Lastconnecttime = DateTime.Now.ToOADate();
		ExecuteTakeScript(protocol, "Connect", source.ID.Id, destination.ID.Id);
	}

	private void PerformDisconnect(SLProtocolExt protocol, DomInstance destination)
	{
		protocol.Lastdisconnecttime = DateTime.Now.ToOADate();
		ExecuteTakeScript(protocol, "Disconnect", default, destination.ID.Id);
	}

	private void ExecuteTakeScript(SLProtocolExt protocol, string action, Guid source, Guid destination)
	{
		var dms = protocol.GetDms();
		var script = dms.GetScript("ControlSurface_Take");

		var parameters = new[]
		{
			new DmsAutomationScriptParamValue("Action", JsonConvert.SerializeObject(new[] { action })),
			new DmsAutomationScriptParamValue("Source", JsonConvert.SerializeObject(source != default ? new[] { source } : Array.Empty<Guid>())),
			new DmsAutomationScriptParamValue("Destinations", JsonConvert.SerializeObject(destination != default ? new[] { destination } : Array.Empty<Guid>())),
			new DmsAutomationScriptParamValue("Levels", JsonConvert.SerializeObject(Array.Empty<Guid>())),
		};
		var dummies = Enumerable.Empty<DmsAutomationScriptDummyValue>();

		script.ExecuteAsync(parameters, dummies);
	}

	private void GetVirtualSignalGroups(SLProtocolExt protocol, out DomInstance srcVsg, out DomInstance dstVsg)
	{
		var parameters = (object[])protocol.GetParameters(new uint[] { Parameter.sourcevsgname, Parameter.destinationvsgname });
		var sourceVsgName = Convert.ToString(parameters[0]);
		var destinationVsgName = Convert.ToString(parameters[1]);

		var domHelper = new DomHelper(protocol.SLNet.SendMessages, "(slc)virtualsignalgroup");

		srcVsg = GetVirtualSignalGroup(domHelper, sourceVsgName);
		dstVsg = GetVirtualSignalGroup(domHelper, destinationVsgName);
	}

	private DomInstance GetVirtualSignalGroup(DomHelper domHelper, string name)
	{
		var filter = DomInstanceExposers.Name.Equal(name);

		var instance = domHelper.DomInstances.Read(filter).SingleOrDefault()
			?? throw new Exception($"Couldn't find VSG with name '{name}'");

		return instance;
	}
}
