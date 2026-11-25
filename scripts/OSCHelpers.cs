using Godot;
using Godot.Collections;
using LucHeart.CoreOSC;

namespace VoyVivika.OSCRelay;

public static class OSCHelpers
{
    public static Array<Array<Variant>> ConvertToGodot(object?[] args)
    {
        Array<Array<Variant>> vs = new Array<Array<Variant>>();
        foreach (var arg in args)
        {
            switch (arg)
            {
                case int i:
                    vs.Add(new Array<Variant> {"i", i});
                    break;
                case float f:
                    vs.Add(new Array<Variant>  {"f", f});
                    break;
                case string str:
                    vs.Add(new Array<Variant>  {"s", str});
                    break;
                case byte[] by:
                    vs.Add(new Array<Variant> {"b", by});
                    break;
                case long l:
                    vs.Add(new Array<Variant> {"h", l});
                    break;
                case ulong ul:
                    vs.Add(new Array<Variant> {"t", ul});
                    break;
                case TimeTag timeTag:
                    vs.Add(new Array<Variant> {"t", timeTag.Tag});
                    break;
                case double d:
                    if (double.IsPositiveInfinity(d))
                    {
                        vs.Add(new Array<Variant> {"I", d});
                        break;
                    }
                    vs.Add(new Array<Variant> {"d", d});
                    break;
                case Symbol symbol:
                    vs.Add(new Array<Variant> { "S", symbol.Value});
                    break;
                case char c:
                    vs.Add(new Array<Variant> {"c", c});
                    break;
                case RGBA rgba:
                    vs.Add(new Array<Variant> {"r", rgba.ToBytes()});
                    break;
                case bool b:
                    vs.Add(new Array<Variant> {b ? "T" : "F", b});
                    break;
                case null:
                    vs.Add(new Array<Variant> {"N", Variant.From<GodotObject>(null)});
                    break;
                case Midi m:
                    byte[] midiData = {m.Data1, m.Data2, m.Port, m.Status};
                    vs.Add(new Array<Variant> {"m", midiData});
                    break;
            }
        }
			
        return vs;
    }
}