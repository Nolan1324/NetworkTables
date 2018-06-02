﻿using FRC.NetworkTables.Interop;
using System;
using System.Runtime.InteropServices;
using FRC.NetworkTables.Strings;

namespace FRC.NetworkTables
{
    public readonly struct ManagedValue : IEquatable<ManagedValue>
    {
        public readonly NtType Type;
        public readonly ulong LastChange;
        public readonly EntryUnion Data;

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + Type.GetHashCode();
                switch (Type)
                {
                    case NtType.Boolean:
                        hash = (hash * 7) + Data.VBoolean.GetHashCode();
                        break;
                    case NtType.Double:
                        hash = (hash * 7) + Data.VDouble.GetHashCode();
                        break;
                    case NtType.String:
                        hash = (hash * 7) + Data.VString.GetHashCode();
                        break;
                    case NtType.Rpc:
                    case NtType.Raw:
                        hash = (hash * 7) + Data.VRaw.GetHashCode();
                        break;
                    case NtType.BooleanArray:
                        hash = (hash * 7) + Data.VBooleanArray.GetHashCode();
                        break;
                    case NtType.DoubleArray:
                        hash = (hash * 7) + Data.VDoubleArray.GetHashCode();
                        break;
                    case NtType.StringArray:
                        hash = (hash * 7) + Data.VStringArray.GetHashCode();
                        break;
                }
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is ManagedValue v)
            {
                return Equals(v);
            }
            return false;
        }

        public bool Equals(ManagedValue other)
        {
            if (Type != other.Type) return false;
            switch (Type)
            {
                case NtType.Unassigned:
                    return true;
                case NtType.Boolean:
                    return Data.VBoolean == other.Data.VBoolean;
                case NtType.Double:
                    return Data.VDouble == other.Data.VDouble;
                case NtType.String:
                    return Data.VString.Span.SequenceEqual(other.Data.VString.Span);
                case NtType.Raw:
                case NtType.Rpc:
                    return Data.VRaw.Span.SequenceEqual(other.Data.VRaw.Span);
                case NtType.BooleanArray:
                    return Data.VBooleanArray.Span.SequenceEqual(other.Data.VBooleanArray.Span);
                case NtType.DoubleArray:
                    return Data.VDoubleArray.Span.SequenceEqual(other.Data.VDoubleArray.Span);
                case NtType.StringArray:
                    return Data.VStringArray.Span.SequenceEqual(other.Data.VStringArray.Span);
                default:
                    return false;
            }
        }

        internal unsafe void CreateNativeFromManaged(NtValue* value)
        {
            value->last_change = LastChange;
            value->type = Type;
            switch (Type)
            {
                case NtType.Boolean:
                    value->data.v_boolean = Data.VBoolean;
                    break;
                case NtType.Double:
                    value->data.v_double = Data.VDouble;
                    break;
                case NtType.String:
                    Utilities.CreateNtString(Data.VString, &value->data.v_string);
                    break;
                case NtType.Rpc:
                case NtType.Raw:
                    value->data.v_raw.str = (byte*)Marshal.AllocHGlobal(Data.VRaw.Length);
                    value->data.v_raw.len = (UIntPtr)Data.VRaw.Length;
                    var rSpan = Data.VRaw.Span;
                    for (int i = 0; i < rSpan.Length; i++)
                    {
                        value->data.v_raw.str[i] = rSpan[i];
                    }
                    break;
                case NtType.BooleanArray:
                    value->data.arr_boolean.arr = (NtBool*)Marshal.AllocHGlobal(Data.VBooleanArray.Length * sizeof(NtBool));
                    value->data.arr_boolean.len = (UIntPtr)Data.VBooleanArray.Length;
                    var bSpan = Data.VBooleanArray.Span;
                    for (int i = 0; i < bSpan.Length; i++)
                    {
                        value->data.arr_boolean.arr[i] = bSpan[i];
                    }
                    break;
                case NtType.DoubleArray:
                    value->data.arr_double.arr = (double*)Marshal.AllocHGlobal(Data.VDoubleArray.Length * sizeof(double));
                    value->data.arr_double.len = (UIntPtr)Data.VDoubleArray.Length;
                    var dSpan = Data.VDoubleArray.Span;
                    for (int i = 0; i < dSpan.Length; i++)
                    {
                        value->data.arr_double.arr[i] = dSpan[i];
                    }
                    break;
                case NtType.StringArray:
                    value->data.arr_string.arr = (NtString*)Marshal.AllocHGlobal(Data.VStringArray.Length * sizeof(NtString));
                    value->data.arr_string.len = (UIntPtr)Data.VStringArray.Length;
                    var sSpan = Data.VStringArray.Span;
                    for (int i = 0; i < sSpan.Length; i++)
                    {
                        Utilities.CreateNtString(sSpan[i], &value->data.arr_string.arr[i]);
                    }
                    break;
            }
        }

        public unsafe static void DisposeCreatedNative(NtValue* v)
        {
            switch (v->type)
            {
                case NtType.String:
                    Utilities.DisposeNtString(&v->data.v_string);
                    break;
                case NtType.Rpc:
                case NtType.Raw:
                    Marshal.FreeHGlobal((IntPtr)v->data.v_raw.str);
                    break;
                case NtType.BooleanArray:
                    Marshal.FreeHGlobal((IntPtr)v->data.arr_boolean.arr);
                    break;
                case NtType.DoubleArray:
                    Marshal.FreeHGlobal((IntPtr)v->data.arr_double.arr);
                    break;
                case NtType.StringArray:
                    int len = (int)v->data.arr_string.len;
                    for (int i = 0; i < len; i++)
                    {
                        Utilities.DisposeNtString(&v->data.arr_string.arr[i]);
                    }
                    Marshal.FreeHGlobal((IntPtr)v->data.arr_string.arr);
                    break;
            }
        }

        internal unsafe ManagedValue(NtValue* v)
        {
            LastChange = v->last_change;
            Type = v->type;
            Data = new EntryUnion(v);
        }

        internal unsafe ManagedValue(in RefManagedValue v)
        {
            LastChange = v.LastChange;
            Type = v.Type;
            Data = new EntryUnion(v);
        }

        internal unsafe ManagedValue(bool v, ulong t)
        {
            LastChange = t;
            Type = NtType.Boolean;
            Data = new EntryUnion(v);
        }

        internal unsafe ManagedValue(double v, ulong t)
        {
            LastChange = t;
            Type = NtType.Double;
            Data = new EntryUnion(v);
        }

        internal unsafe ManagedValue(ReadOnlySpan<char> v, ulong t)
        {
            LastChange = t;
            Type = NtType.String;
            Data = new EntryUnion(v);
        }

        internal unsafe ManagedValue(ReadOnlySpan<byte> v, ulong t)
        {
            LastChange = t;
            Type = NtType.Raw;
            Data = new EntryUnion(v);
        }

        internal unsafe ManagedValue(ReadOnlySpan<byte> v, ulong t, bool r)
        {
            LastChange = t;
            Type = r ? NtType.Rpc : NtType.Raw;
            Data = new EntryUnion(v);
        }

        internal unsafe ManagedValue(ReadOnlySpan<bool> v, ulong t)
        {
            LastChange = t;
            Type = NtType.BooleanArray;
            Data = new EntryUnion(v);
        }

        internal unsafe ManagedValue(ReadOnlySpan<double> v, ulong t)
        {
            LastChange = t;
            Type = NtType.DoubleArray;
            Data = new EntryUnion(v);
        }

        internal unsafe ManagedValue(ReadOnlySpan<string> v, ulong t)
        {
            LastChange = t;
            Type = NtType.StringArray;
            Data = new EntryUnion(v);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public readonly struct EntryUnion
    {
        [FieldOffset(0)]
        public readonly bool VBoolean;
        [FieldOffset(0)]
        public readonly double VDouble;
        [FieldOffset(8)]
        public readonly ReadOnlyMemory<char> VString;
        [FieldOffset(8)]
        public readonly ReadOnlyMemory<byte> VRaw;
        [FieldOffset(8)]
        public readonly ReadOnlyMemory<bool> VBooleanArray;
        [FieldOffset(8)]
        public readonly ReadOnlyMemory<double> VDoubleArray;
        [FieldOffset(8)]
        public readonly ReadOnlyMemory<string> VStringArray;

        internal EntryUnion(bool v)
        {
            VBoolean = false;
            VDouble = 0;
            VString = null;
            VRaw = null;
            VBooleanArray = null;
            VDoubleArray = null;
            VStringArray = null;

            VBoolean = v;
        }

        internal EntryUnion(double v)
        {
            VBoolean = false;
            VDouble = 0;
            VString = null;
            VRaw = null;
            VBooleanArray = null;
            VDoubleArray = null;
            VStringArray = null;

            VDouble = v;
        }

        internal EntryUnion(ReadOnlySpan<char> v)
        {
            VBoolean = false;
            VDouble = 0;
            VString = null;
            VRaw = null;
            VBooleanArray = null;
            VDoubleArray = null;
            VStringArray = null;

            VString = v.ToArray();
        }

        internal EntryUnion(ReadOnlySpan<byte> v)
        {
            VBoolean = false;
            VDouble = 0;
            VString = null;
            VRaw = null;
            VBooleanArray = null;
            VDoubleArray = null;
            VStringArray = null;

            VRaw = v.ToArray();
        }

        internal EntryUnion(ReadOnlySpan<bool> v)
        {
            VBoolean = false;
            VDouble = 0;
            VString = null;
            VRaw = null;
            VBooleanArray = null;
            VDoubleArray = null;
            VStringArray = null;

            VBooleanArray = v.ToArray();
        }

        internal EntryUnion(ReadOnlySpan<double> v)
        {
            VBoolean = false;
            VDouble = 0;
            VString = null;
            VRaw = null;
            VBooleanArray = null;
            VDoubleArray = null;
            VStringArray = null;

            VDoubleArray = v.ToArray();
        }

        internal EntryUnion(ReadOnlySpan<string> v)
        {
            VBoolean = false;
            VDouble = 0;
            VString = null;
            VRaw = null;
            VBooleanArray = null;
            VDoubleArray = null;
            VStringArray = null;

            VStringArray = v.ToArray();
        }

        internal unsafe EntryUnion(NtValue* v)
        {
            VBoolean = false;
            VDouble = 0;
            VString = null;
            VRaw = null;
            VBooleanArray = null;
            VDoubleArray = null;
            VStringArray = null;

            switch (v->type)
            {
                case NtType.Unassigned:
                    break;
                case NtType.Boolean:
                    VBoolean = v->data.v_boolean.Get();
                    break;
                case NtType.Double:
                    VDouble = v->data.v_double;
                    break;
                case NtType.String:
                    VString = UTF8String.ReadUTF8String(v->data.v_string.str, v->data.v_string.len).AsMemory();
                    break;
                case NtType.Rpc:
                case NtType.Raw:
                    var raw  = new byte[(int)v->data.v_raw.len];
                    for (int i = 0; i < raw.Length; i++)
                    {
                        raw[i] = v->data.v_raw.str[i];
                    }
                    VRaw = raw;
                    break;
                case NtType.BooleanArray:
                    var barr = new bool[(int)v->data.arr_boolean.len];
                    for (int i = 0; i < barr.Length; i++)
                    {
                        barr[i] = v->data.arr_boolean.arr[i].Get();
                    }
                    VBooleanArray = barr;
                    break;
                case NtType.DoubleArray:
                    var darr = new double[(int)v->data.arr_double.len];
                    for (int i = 0; i < darr.Length; i++)
                    {
                        darr[i] = v->data.arr_double.arr[i];
                    }
                    VDoubleArray = darr;
                    break;
                case NtType.StringArray:
                    var sarr = new string[(int)v->data.arr_string.len];
                    for (int i = 0; i < sarr.Length; i++)
                    {
                        sarr[i] = UTF8String.ReadUTF8String(v->data.arr_string.arr[i].str, v->data.arr_string.arr[i].len);
                    }
                    VStringArray = sarr;
                    break;
            }
        }

        internal EntryUnion(in RefManagedValue v)
        {
            VBoolean = false;
            VDouble = 0;
            VString = null;
            VRaw = null;
            VBooleanArray = null;
            VDoubleArray = null;
            VStringArray = null;

            switch (v.Type)
            {
                case NtType.Unassigned:
                    break;
                case NtType.Boolean:
                    VBoolean = v.Data.VBoolean;
                    break;
                case NtType.Double:
                    VDouble = v.Data.VDouble;
                    break;
                case NtType.String:
                    VString = v.Data.VString.ToArray();
                    break;
                case NtType.Rpc:
                case NtType.Raw:
                    VRaw = v.Data.VRaw.ToArray();
                    break;
                case NtType.BooleanArray:
                    VBooleanArray = v.Data.VBooleanArray.ToArray();
                    break;
                case NtType.DoubleArray:
                    VDoubleArray = v.Data.VDoubleArray.ToArray();
                    break;
                case NtType.StringArray:
                    VStringArray = v.Data.VStringArray.ToArray();
                    break;
            }
        }
    }
}
