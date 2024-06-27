using System;
using System.Threading.Tasks;

using QAction_2;

using Skyline.DataMiner.FlowEngineering.Protocol;
using Skyline.DataMiner.FlowEngineering.Protocol.DCF;
using Skyline.DataMiner.FlowEngineering.Protocol.Enums;
using Skyline.DataMiner.FlowEngineering.Protocol.Model;
using Skyline.DataMiner.Scripting;

/// <summary>
/// DataMiner QAction Class: After Startup.
/// </summary>
public static class QAction
{
	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Run(SLProtocolExt protocol)
	{
		try
		{
			// make sure all old cached data is removed
			FlowEngineeringManagerInstances.CreateNewInstance(protocol);

			FillInterfacesTable(protocol);
			UpdateFleInterfaces(protocol);
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}

	private static void FillInterfacesTable(SLProtocolExt protocol)
	{
		var table = new InterfacestableQActionRow[]
		{
			new InterfacestableQActionRow
			{
				Interfacestableid = "1",
				Interfacestablename = "Ethernet 1",
			},
			new InterfacestableQActionRow
			{
				Interfacestableid = "2",
				Interfacestablename = "Ethernet 2",
			},
		};

		protocol.interfacestable.FillArray(table);
	}

	private static async void UpdateFleInterfaces(SLProtocolExt protocol)
	{
		await WaitUntilDcfInterfacesAreCreated(protocol, 2);

		var flowEngineering = FlowEngineeringManager.GetInstance(protocol);
		var dcfInterfaceHelper = DcfInterfaceHelper.Create(protocol);

		flowEngineering.Interfaces.Clear();
		flowEngineering.Interfaces.Add(CreateInterface(1, "Ethernet 1", dcfInterfaceHelper));
		flowEngineering.Interfaces.Add(CreateInterface(2, "Ethernet 2", dcfInterfaceHelper));

		flowEngineering.Interfaces.UpdateTable(protocol);
	}

	private static Interface CreateInterface(int id, string name, DcfInterfaceHelper dcfInterfaceHelper)
	{
		var key = Convert.ToString(id);

		var intf = new Interface(key)
		{
			Description = name,
			DisplayKey = name,
			Type = InterfaceType.Ethernet,
			AdminStatus = InterfaceAdminStatus.Up,
			OperationalStatus = InterfaceOperationalStatus.Up,
		};

		if (dcfInterfaceHelper.TryFindInterface(1, key, out var dcfIntf))
		{
			intf.DcfInterfaceId = dcfIntf.ID;
		}

		return intf;
	}

	private static async Task WaitUntilDcfInterfacesAreCreated(SLProtocolExt protocol, int expectedInterfacesCount)
	{
		await WaitHelper.WaitUntilAsync(
			() => protocol.RowCount(65049) >= expectedInterfacesCount,
			TimeSpan.FromSeconds(30));
	}
}
