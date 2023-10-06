namespace Skyline.DataMiner.FlowEngineering.Protocol
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Core.DataMinerSystem.Protocol;
	using Skyline.DataMiner.FlowEngineering.Protocol.Model;
	using Skyline.DataMiner.Scripting;

	public class DcfInterfaceHelper
    {
        private readonly Dictionary<int, DcfInterface> _interfaces = new Dictionary<int, DcfInterface>();

        private DcfInterfaceHelper()
        {
        }

        public static DcfInterfaceHelper Create(SLProtocol protocol)
        {
            var intfs = protocol.GetLocalElement()
                .GetTable(65049)
                .GetColumns(
                    new uint[] { 0, 1, 5, },
                    (string idx, string name, string dynamicLink) =>
                    {
                        var id = Convert.ToInt32(idx);

                        var intf = new DcfInterface(id)
                        {
                            Name = name,
                            DynamicLink = dynamicLink,
                        };

                        return intf;
                    }
                );

            var helper = new DcfInterfaceHelper();

            foreach (var intf in intfs)
            {
                helper._interfaces.Add(intf.ID, intf);
            }

            return helper;
        }

        public bool TryFindInterface(int groupId, out DcfInterface dcfInterface)
        {
            return _interfaces.TryGetValue(groupId, out dcfInterface);
        }

        public bool TryFindInterface(int groupId, string dynamicPk, out DcfInterface dcfInterface)
        {
            foreach (var intf in _interfaces.Values)
            {
                var parts = intf.DynamicLink.Split(';');
                if (parts.Length < 2)
                    continue;

                if (Convert.ToInt32(parts[0]) == groupId && String.Equals(parts[1], dynamicPk))
                {
                    dcfInterface = intf;
                    return true;
                }
            }

            dcfInterface = null;
            return false;
        }
    }
}
