namespace Skyline.DataMiner.FlowEngineering.Protocol.Model
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.FlowEngineering.Protocol;

	public class Flows<T> : Dictionary<string, T>
        where T : Flow
    {
        protected FlowEngineeringManager _manager;

        protected Flows(FlowEngineeringManager manager)
        {
            _manager = manager;
        }

        public void Add(T flow)
        {
            if (flow == null)
            {
                throw new ArgumentNullException(nameof(flow));
            }

            Add(flow.Instance, flow);
        }

        public void AddRange(IEnumerable<T> flows)
        {
            if (flows == null)
            {
                throw new ArgumentNullException(nameof(flows));
            }

            foreach (var flow in flows)
            {
                Add(flow);
            }
        }

        public void ReplaceFlows(IEnumerable<T> newFlows)
        {
            Clear();
            AddRange(newFlows);
        }
    }
}