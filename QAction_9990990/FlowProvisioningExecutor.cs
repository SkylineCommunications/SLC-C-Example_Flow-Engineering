namespace QAction_9990990
{
	using System;

	using Skyline.DataMiner.ConnectorAPI.FlowEngineering.Enums;
	using Skyline.DataMiner.ConnectorAPI.FlowEngineering.Info;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
	using Skyline.DataMiner.Core.InterAppCalls.Common.MessageExecution;
	using Skyline.DataMiner.FlowEngineering.Protocol;
	using Skyline.DataMiner.Scripting;

	public class FlowProvisioningExecutor : SimpleMessageExecutor<FlowInfoMessage>
	{
		public FlowProvisioningExecutor(FlowInfoMessage message) : base(message)
		{
			// Nothing to do
		}

		public override bool TryExecute(object dataSource, object dataDestination, out Message optionalReturnMessage)
		{
			var protocol = (SLProtocolExt)dataDestination;

			// Flow engineering
			var flowEngineering = FlowEngineeringManager.GetInstance(protocol);

			switch (Message.ActionType)
			{
				case ActionType.Create:
					var flowInstance = Message.OptionalDestinationIdentifier;
					var addedFlows = flowEngineering.RegisterFlowEngineeringFlowsFromInterAppMessage(protocol, Message, flowInstance);

					break;

				case ActionType.Delete:
					var removedFlows = flowEngineering.UnregisterFlowEngineeringFlowsFromInterAppMessage(protocol, Message);

					break;

				default:
					throw new InvalidOperationException($"Unknown action: {Message.ActionType}");
			}

			optionalReturnMessage = null;
			return true;
		}
	}
}
