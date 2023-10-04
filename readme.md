# Flow Engineering Connector Example

Example DataMiner connector that demonstrates how to support generic flow engineering tables via InterApp messages.

## Implementation
### Protocol.xml

 - Copy tables 1000000, 1000100 and 1000200
 - Copy relations
 - Copy QAction 1000000 (including code)

### Skyline.DataMiner.FlowEngineering.Protocol namespace

[Link to code](../QAction_1/Skyline/DataMiner/FlowEngineering/Protocol)

### FLE Interfaces table

todo

### Incoming and Outgoing Flows table

todo

### Process InterApp messages

See [QAction 9000000](../QAction_9000000/QAction_9000000.cs)

## Parameters

### FLE Interfaces Overview Table
> PID: 1000000

List of interfaces that are eligible for flow engineering.

| IDX | PID     | Description                | Values              | Explanation |
| --- | ------- | -------------------------- | ------------------- | ----------- |
| 0   | 1000001 | Index                      | String              | Unique key of the table row.
| 1   | 1000002 | Description                | String              | Description of the interface.
| 2   | 1000003 | Type                       | Ethernet/SDI/ASI    | Type of the interface.
| 3   | 1000004 | Admin Status               | Up/Down/Testing     | Admin status.
| 4   | 1000005 | Oper. Status               | Up/Down/Testing/... | Operational status.
| 5   | 1000006 | Display Key [IDX]          | String              | Display key.
| 6   | 1000007 | Rx Bitrate                 | Number (Mbps)       | Rx bitrate on this interface as reported by the device.
| 7   | 1000008 | Rx Flows                   | Number (Flows)      | Total number of flows in [FLE Incoming Flows Table](#fle-incoming-flows-table) that are present.
| 8   | 1000009 | Tx Bitrate                 | Number (Mbps)       | Tx bitrate on this interface as reported by the device.
| 9   | 1000010 | Tx Flows                   | Number (Flows)      | Total number of flows in [FLE Outgoing Flows Table](#fle-outgoing-flows-table) that are present.
| 10  | 1000011 | Rx Utilization             | Number (%)          | Utilization of the interface for Rx traffic.
| 11  | 1000012 | Tx Utilization             | Number (%)          | Utilization of the interface for Tx traffic.
| 12  | 1000013 | Expected Rx Bitrate        | Number (Mbps)       | Sum of all expected bitrates in [FLE Incoming Flows Table](#fle-incoming-flows-table).
| 13  | 1000014 | Expected Rx Bitrate Status | Normal/Low/High     | Status of 'Rx Bitrate' compared to 'Expected Rx Bitrate'.
| 14  | 1000015 | Expected Rx Flows          | Number (Flows)      | Total number of flows in [FLE Incoming Flows Table](#fle-incoming-flows-table).
| 15  | 1000016 | Expected Rx Flows Status   | Normal/Low/High     | Status of 'v Flows' compared to 'Expected Rx Flows'.
| 16  | 1000017 | Expected Tx Bitrate        | Number (Mbps)       | Sum of all expected bitrates in [FLE Outgoing Flows Table](#fle-outgoing-flows-table).
| 17  | 1000018 | Expected Tx Bitrate Status | Normal/Low/High     | Status of 'Tx Bitrate' compared to 'Expected Tx Bitrate'.
| 18  | 1000019 | Expected Tx Flows          | Number (Flows)      | Total number of flows in [FLE Outgoing Flows Table](#fle-outgoing-flows-table).
| 19  | 1000020 | Expected Tx Flows Status   | Normal/Low/High     | Status of 'Tx Flows' compared to 'Expected Tx Flows'.
| 20  | 1000021 | DCF Interface ID           | Number              | Link to the DCF interface in general table 65049

### FLE Incoming Flows Table
> PID: 1000100

List of all incoming flows on the device.

| IDX | PID     | Description                | Value                         | Explanation |
| --- | ------- | -------------------------- | ----------------------------- | ----------- |
| 0   | 1000101 | Instance [IDX]             | String                        | Unique key of the table row.
| 1   | 1000102 | Destination IP             | String                        | Multicast destination IP address. Empty for SDI and ASI.
| 2   | 1000103 | Destination Port           | Number                        | Multicast destination port. Empty for SDI and ASI.
| 3   | 1000104 | Source IP                  | String                        | Multicast source IP address. Empty for SDI and ASI.
| 4   | 1000105 | Incoming Interface         | String                        | Foreign key to [FLE Interfaces Overview Table](#fle-interfaces-overview-table).
| 5   | 1000106 | Transport Type             | IP/SDI/ASI                    | Transport type of the signal.
| 6   | 1000107 | Rx Bitrate                 | Number (Mbps)                 | Actual received bitrate of the flow (as reported by the device).
| 7   | 1000108 | Expected Rx Bitrate        | Number (Mbps)                 | Expected received bitrate of the flow (from FLE).
| 8   | 1000109 | Expected Rx Bitrate Status | Normal/Low/High               | Status of 'Rx Bitrate' compared to 'Expected Rx Bitrate'.
| 9   | 1000110 | Label                      | String                        | Custom label.
| 10  | 1000111 | FK Outgoing                | String                        | Foreign key to [FLE Outgoing Flows Table](#fle-outgoing-flows-table). Only use this in case of 1-N mapping between incoming and outgoing, otherwise keep empty.
| 11  | 1000112 | Linked Flow                | String (GUID)                 | GUID of the linked source flow. Empty for 'Local System' flows.
| 12  | 1000113 | Flow Owner                 | Local System/Flow Engineering | Local System: Flows that exist on the device, but not provisioned by FLE.<br>Flow Engineering: Flows that are provisioned by FLE.
| 13  | 1000114 | Present                    | No/Yes                        | Indicates if the flow is present on the system or not.

### FLE Outgoing Flows Table
> PID: 1000200

List of all outgoing flows on the device.

| IDX | PID     | Description                | Value                         | Explanation |
| --- | ------- | -------------------------- | ----------------------------- | ----------- |
| 0   | 1000201 | Instance [IDX]             | String                        | Unique key of the table row.
| 1   | 1000202 | Destination IP             | String                        | Multicast destination IP address. Empty for SDI and ASI.
| 2   | 1000203 | Destination Port           | Number                        | Multicast destination port. Empty for SDI and ASI.
| 3   | 1000204 | Source IP                  | String                        | Multicast source IP address. Empty for SDI and ASI.
| 4   | 1000205 | Incoming Interface         | String                        | Foreign key to [FLE Interfaces Overview Table](#fle-interfaces-overview-table).
| 5   | 1000206 | Transport Type             | IP/SDI/ASI                    | Transport type of the signal.
| 6   | 1000207 | Tx Bitrate                 | Number (Mbps)                 | Actual transmitted bitrate of the flow (as reported by the device).
| 7   | 1000208 | Expected Tx Bitrate        | Number (Mbps)                 | Expected transmitted bitrate of the flow (from FLE).
| 8   | 1000209 | Expected Tx Bitrate Status | Normal/Low/High               | Status of 'Tx Bitrate' compared to 'Expected Tx Bitrate'.
| 9   | 1000210 | Label                      | String                        | Custom label.
| 10  | 1000211 | FK Outgoing                | String                        | Foreign key to [FLE Incoming Flows Table](#fle-incoming-flows-table). Only use this in case of N-1 mapping between incoming and outgoing, otherwise keep empty.
| 11  | 1000212 | Linked Flow                | String (GUID)                 | GUID of the linked source flow. Empty for 'Local System' flows.
| 12  | 1000213 | Flow Owner                 | Local System/Flow Engineering | Local System: Flows that exist on the device, but not provisioned by FLE.<br>Flow Engineering: Flows that are provisioned by FLE.
| 13  | 1000214 | Present                    | No/Yes                        | Indicates if the flow is present on the system or not.

## Example connections
### IP to IP
Incoming:

| Instance | Destination IP | Source IP  | Interface | FK to Out |
| -------- | -------------- | ---------- | --------- | --------- |
| X        | 239.0.0.1      | 10.1.1.2   | Eth1      |           |

Outgoing:

| Instance | Destination IP | Source IP  | Interface | FK to In  |
| -------- | -------------- | ---------- | --------- | --------- |
| Y        | 239.0.0.1      | 10.1.1.2   | Eth2      | X         |

### IP to SDI
Incoming:

| Instance | Destination IP | Source IP  | Interface | FK to Out |
| -------- | -------------- | ---------- | --------- | --------- |
| X        | 239.0.0.1      | 10.1.1.2   | Eth1      |           |

Outgoing:

| Instance | Destination IP | Source IP  | Interface | FK to In  |
| -------- | -------------- | ---------- | --------- | --------- |
| Y        |                |            | SDI 2     | X         |

### SDI to SDI
Incoming:

| Instance | Destination IP | Source IP  | Interface | FK to Out |
| -------- | -------------- | ---------- | --------- | --------- |
| X        |                |            | SDI 1     | Y         |

Outgoing:

| Instance | Destination IP | Source IP  | Interface | FK to In  |
| -------- | -------------- | ---------- | --------- | --------- |
| Y        |                |            | SDI 2     | X         |

### SDI to IP
Incoming:

| Instance | Destination IP | Source IP  | Interface | FK to Out |
| -------- | -------------- | ---------- | --------- | --------- |
| X        |                |            | SDI 1     |           |

Outgoing:

| Instance | Destination IP | Source IP  | Interface | FK to In  |
| -------- | -------------- | ---------- | --------- | --------- |
| Y        | 239.0.0.1      | 10.1.1.2   | Eth2      | X         |



