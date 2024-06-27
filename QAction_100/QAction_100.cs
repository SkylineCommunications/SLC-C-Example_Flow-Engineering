using System;
using System.Linq;

using Newtonsoft.Json;

using QAction_100;

using Skyline.DataMiner.Core.DataMinerSystem.Common;
using Skyline.DataMiner.Core.DataMinerSystem.Protocol;
using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
using Skyline.DataMiner.Net.Messages.SLDataGateway;
using Skyline.DataMiner.Scripting;

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
			var dms = protocol.GetDms();

			var domHelper = new DomHelper(protocol.SLNet.SendMessages, "(slc)virtualsignalgroup");
			var source = GetVirtualSignalGroup(domHelper, "LA CAM-01");
			var destination = GetVirtualSignalGroup(domHelper, "FLE Test 1");

			switch (_nextAction)
			{
				case ControlSurfaceAction.Connect:
					PerformConnect(protocol, dms, source, destination);
					_nextAction= ControlSurfaceAction.Disconnect;
					break;
				case ControlSurfaceAction.Disconnect:
				default:
					PerformDisconnect(protocol, dms, destination);
					_nextAction= ControlSurfaceAction.Connect;
					break;
			}
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}

	private void PerformConnect(SLProtocolExt protocol, IDms dms, DomInstance source, DomInstance destination)
	{
		protocol.Lastconnecttime = DateTime.Now.ToOADate();
		ExecuteTakeScript(dms, "Connect", source.ID.Id, destination.ID.Id);
	}

	private void PerformDisconnect(SLProtocolExt protocol, IDms dms, DomInstance destination)
	{
		protocol.Lastdisconnecttime = DateTime.Now.ToOADate();
		ExecuteTakeScript(dms, "Disconnect", default, destination.ID.Id);
	}

	private void ExecuteTakeScript(IDms dms, string action, Guid source, Guid destination)
	{
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

	private DomInstance GetVirtualSignalGroup(DomHelper domHelper, string name)
	{
		var filter = DomInstanceExposers.Name.Equal(name);
		return domHelper.DomInstances.Read(filter).FirstOrDefault();
	}
}
