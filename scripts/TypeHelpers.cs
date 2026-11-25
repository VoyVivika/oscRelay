using Godot;
using System;
using System.ComponentModel;

namespace VoyVivika.OSCRelay
{
    
    public static class TypeHelpers
    {
        public enum TYPES
        {
            // Used for the Graph Node Types
            
            // I don't think this program is going to support most of these
            // this program is mostly meant for VRChat and ChilloutVR support
            [Description("Invalid")]
            Inval = -1,
            [Description("Int32")]
            i = 0, // int32
            [Description("Float")]
            f = 1, // float32
            [Description("String")]
            s = 2, // string
            [Description("Blob (Byte Array)")]
            b = 3, // blob (byte array)
            [Description("Int64 (Long)")]
            h = 4, // int64 (long)
            [Description("Timetag (UInt64 / Unsigned Long")]
            t = 5, // timetag (uint64 / unsigned long)
            [Description("Float64 (Double)")]
            d = 6, // float64 (double)
            [Description("Symbol (Stored as String")]
            S = 7, // symbol (stored as string)
            [Description("Char")]
            c = 8, // char
            [Description("32-bit RGBA (as Byte Array)")]
            r = 9, // 32 bit RGBA color as byte array
            [Description("Boolean")]
            Boolean = 10, // boolean, duh
            [Description("Midi (4 Length Byte Array)")]
            m = 11, // 32 bit midi message as a byte array
            [Description("Infinity")]
            I = 12, // infinity
            [Description("Null, NaN, Nil, etc.")]
            N = 13, // null
            [Description("Known as Value, not known what type")]
            Value = 14,
            [Description("Output Node for Continuing a Connection")]
            Flow = 15
            
        }

        public static String GetTypeName(TYPES type)
        {
            switch (type)
            {
                
                case TYPES.Inval:
                    return "Invalid Type";
                case TYPES.i:
                    return "Integer";
                case TYPES.f:
                    return "Float";
                case TYPES.s:
                    return "String";
                case TYPES.b:
                    return "Blob";
                case TYPES.h:
                    return "Long";
                case TYPES.t:
                    return "Timetag";
                case TYPES.d:
                    return "Double";
                case TYPES.S:
                    return "Symbol";
                case TYPES.c:
                    return "Char";
                case TYPES.r:
                    return "RGBA";
                case TYPES.Boolean:
                    return "Boolean";
                case TYPES.m:
                    return "MIDI";
                case TYPES.I:
                    return "Impulse/Infinitum";
                case TYPES.N:
                    return "Null";
                case TYPES.Value:
                    return "Value";
                case TYPES.Flow:
                    return "Control Flow";
            }

            return "Unknown or Invalid";
        }

        public static Color GetColor(TYPES type)
        {
            switch (type)
            {
                case TYPES.i:
                    return Colors.Aqua;
                case TYPES.f:
                    return Colors.LawnGreen;
                case TYPES.s:
                    return Colors.Chocolate;
                case TYPES.b:
                    return Colors.BlueViolet;
                case TYPES.h:
                    return Colors.CornflowerBlue;
                case TYPES.t:
                    return Colors.Burlywood;
                case TYPES.d:
                    return Colors.ForestGreen;
                case TYPES.S:
                    return Colors.Coral;
                case TYPES.c:
                    return Colors.OrangeRed;
                case TYPES.r:
                    return Colors.Red;
                case TYPES.Boolean:
                    return Colors.Gold;
                case TYPES.m:
                    return Colors.MediumPurple;
                case TYPES.I:
                    return Colors.DarkGray;
                case TYPES.N:
                    return Colors.Gray;
                case TYPES.Value:
                    return Colors.HotPink;
                case TYPES.Flow:
                    return Colors.White;
                
            }
            
            return Colors.Black;
        }
    }
}

