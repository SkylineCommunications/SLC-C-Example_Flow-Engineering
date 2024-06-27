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

			try
			{
				switch (Message.ActionType)
				{
					case ActionType.Create:
						UpdateLastDuration(protocol, Parameter.lastconnecttime, Parameter.connectduration);
						var addedFlows = flowEngineering.RegisterProvisionedFlowFromInterAppMessage(protocol, Message);

						break;

					case ActionType.Delete:
						UpdateLastDuration(protocol, Parameter.lastdisconnecttime, Parameter.disconnectduration);
						var (provisionedFlow, removedFlows) = flowEngineering.UnregisterProvisionedFlowFromInterAppMessage(protocol, Message);

						break;

					default:
						throw new InvalidOperationException($"Unknown action: {Message.ActionType}");
				}

				// in reality this should be done after polling and validating the configuration
				Message.ReplySuccess(protocol);
			}
			catch (Exception ex)
			{
				Message.ReplyFailed(protocol, ex.ToString());
				throw;
			}

			optionalReturnMessage = null;
			return true;
		}

		private void UpdateLastDuration(SLProtocolExt protocol, int pidTime, int pidDuration)
		{
			var lastTimeDouble = Convert.ToDouble(protocol.GetParameter(pidTime));
			if (lastTimeDouble <= 0)
			{
				return;
			}

			var lastTime = DateTime.FromOADate(lastTimeDouble);
			var duration = DateTime.Now - lastTime;

			protocol.SetParameter(pidDuration, duration.TotalMilliseconds);
		}

	}
}
