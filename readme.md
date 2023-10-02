# Flow Engineering Connector Example

Example DataMiner connector that demonstrates how to support generic flow engineering tables via InterApp messages.

## Parameters

### FLE Interfaces Overview Table (1000000)
| IDX | PID     | Description 			    |
| --- | ------- | ------------------------- |
| 0   | 1000001 | Index					    |
| 1   | 1000002 | Description			    |
| 2   | 1000003 | Type					    |
| 3   | 1000004 | Admin Status			    |
| 4   | 1000005 | Oper. Status			    |
| 5   | 1000006 | Display Key			    |
| 6   | 1000007 | Rx Bitrate			    |
| 7   | 1000008 | Rx Flows				    |
| 8   | 1000009 | Tx Bitrate			    |
| 9   | 1000010 | Tx Flows				    |
| 10  | 1000011 | Rx Utilization		    |
| 11  | 1000012 | Tx Utilization		    |
| 12  | 1000013 | Expected Rx Bitrate	    |
| 13  | 1000014 | Expected Rx Bitrate State |
| 14  | 1000015 | Expected Rx Flows		    |
| 15  | 1000016 | Expected Rx Flows State   |
| 16  | 1000017 | Expected Tx Bitrate	    |
| 17  | 1000018 | Expected Tx Bitrate State |
| 18  | 1000019 | Expected Tx Flows		    |
| 19  | 1000020 | Expected Tx Flows State   |
| 20  | 1000021 | Dcf Interface ID		    |

### FLE Incoming Flows Table (1000100)
| IDX | PID     | Description 			    |
| --- | ------- | ------------------------- |
| 0   | 1000101 | Instance				    |
| 1   | 1000102 | Destination			    |
| 2   | 1000103 | Destination Port		    |
| 3   | 1000104 | Source				    |
| 4   | 1000105 | Incoming Interface	    |
| 5   | 1000106 | Transport Type		    |
| 6   | 1000107 | Rx Bitrate			    |
| 7   | 1000108 | Expected Rx Bitrate	    |
| 8   | 1000109 | Expected Rx Bitrate State |
| 9   | 1000110 | Label					    |
| 10  | 1000111 | FK Outgoing			    |
| 11  | 1000112 | Linked Flow			    |
| 12  | 1000113 | Flow Owner			    |
| 13  | 1000114 | Present				    |

### FLE Outgoing Flows Table (1000200)
| IDX | PID     | Description 				|
| --- | ------- | -----------------         |
| 0   | 1000201 | Instance					|
| 1   | 1000202 | Destination				|
| 2   | 1000203 | Destination Port			|
| 3   | 1000204 | Source					|
| 4   | 1000205 | Incoming Interface		|
| 5   | 1000206 | Transport Type			|
| 6   | 1000207 | Rx Bitrate				|
| 7   | 1000208 | Expected Rx Bitrate		|
| 8   | 1000209 | Expected Rx Bitrate State	|
| 9   | 1000210 | Label						|
| 10  | 1000211 | FK Outgoing				|
| 11  | 1000212 | Linked Flow				|
| 12  | 1000213 | Flow Owner				|
| 13  | 1000214 | Present					|
