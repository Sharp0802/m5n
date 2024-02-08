namespace M5N.Primitives;

[Flags]
public enum ControlCode : ushort
{
    Syn    = 0b10_000000_00000000,
    SynAck = Syn | Ack,
    Ack    = 0b01_000000_00000000
}