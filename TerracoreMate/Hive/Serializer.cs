﻿using System.Reflection;
using TerracoreMate.Hive.Attributes;
using TerracoreMate.Hive.Models;
using TerracoreMate.Hive.Operations;

namespace TerracoreMate.Hive;

/// <summary>
/// A helper class to handle serialization.
/// </summary>
class Serializer
{
    /// <summary>
    /// Writes a 32 bit variable integer to the provided stream.
    /// </summary>
    /// <param name="oStream">The stream to write to.</param>
    /// <param name="n">The 32 bit integer to write.</param>
    private static void WriteVarint32(MemoryStream oStream, uint n)
    {
        while (n >= 0x80)
        {
            oStream.WriteByte(Convert.ToByte(0x80 | (n & 0x7F)));
            n = n >> 7;
        }

        oStream.WriteByte(Convert.ToByte(n));
    }

    /// <summary>
    /// Adds an object to the provided stream.
    /// </summary>
    /// <param name="oStream">The stream to add the object to.</param>
    /// <param name="obj">The object to add.</param>
    private void AddToStream(MemoryStream oStream, object obj)
    {
        byte[] buf;

        if (obj == null) return;
        switch (obj)
        {
            case bool value:
                oStream.WriteByte((byte)(value ? 1 : 0));
                break;
            case byte value:
                oStream.WriteByte(value);
                break;
            case short value:
                buf = BitConverter.GetBytes(value);
                oStream.Write(buf, 0, buf.Length);
                break;
            case ushort value:
                buf = BitConverter.GetBytes(value);
                oStream.Write(buf, 0, buf.Length);
                break;
            case int value:
                buf = BitConverter.GetBytes(value);
                oStream.Write(buf, 0, buf.Length);
                break;
            case uint value:
                buf = BitConverter.GetBytes(value);
                oStream.Write(buf, 0, buf.Length);
                break;
            case long value:
                buf = BitConverter.GetBytes(value);
                oStream.Write(buf, 0, buf.Length);
                break;
            case ulong value:
                buf = BitConverter.GetBytes(value);
                oStream.Write(buf, 0, buf.Length);
                break;
            case float value:
                buf = BitConverter.GetBytes(value);
                oStream.Write(buf, 0, buf.Length);
                break;
            case double value:
                buf = BitConverter.GetBytes(value);
                oStream.Write(buf, 0, buf.Length);
                break;

            case byte[] value:
                oStream.Write(value, 0, value.Length);
                break;
            case string value:
                if (string.IsNullOrEmpty(value))
                {
                    oStream.WriteByte(0);
                    break;
                }

                buf = System.Text.Encoding.UTF8.GetBytes(value);
                var len = Convert.ToUInt32(buf.Length);
                WriteVarint32(oStream, len);
                oStream.Write(buf, 0, (int)len);
                break;
            case DateTime value:
                buf = BitConverter.GetBytes(Convert.ToUInt32(value.Ticks / 10000000 - 62135596800)); //01.01.1970
                oStream.Write(buf, 0, buf.Length);
                break;
            case Asset value:
                var precision = value.GetPrecision();
                buf = BitConverter.GetBytes(
                    Convert.ToUInt64(Math.Round((double)value.amount * Math.Pow(10, precision))));
                oStream.Write(buf, 0, buf.Length);
                oStream.WriteByte(Convert.ToByte(precision));
                // Convert HIVE & HBD to STEEM & SBD because the blockchain serialization is still based on STEEM & SBD

                string strSymbol = value.symbol == "HIVE" ? "STEEM" : value.symbol == "HBD" ? "SBD" : value.symbol;
                for (var i = 0; i <= 6; i++)
                {
                    oStream.WriteByte(i < strSymbol.Length ? (byte)strSymbol[i] : (byte)0);
                }

                break;
            case PublicKey value:
                if (value == null || value.key == "STM1111111111111111111111111111111114T1Anm")
                {
                    buf = new byte[33];
                    oStream.Write(buf, 0, 33);
                }
                else
                {
                    buf = value.Decode();
                    oStream.Write(buf, 0, buf.Length);
                }

                break;
            case object[] value:
                WriteVarint32(oStream, (uint)value.Length);
                if (value.Length == 0) break;
                foreach (var item in value)
                {
                    Serialize(oStream, item);
                }

                break;
            case Dictionary<PublicKey, ushort> value:
                WriteVarint32(oStream, (uint)value.Count);
                if (value.Count == 0) break;
                foreach (var item in value)
                {
                    Serialize(oStream, item);
                }

                break;

            case Dictionary<string, ushort> value:
                WriteVarint32(oStream, (uint)value.Count);
                if (value.Count == 0) break;
                foreach (KeyValuePair<string, ushort> item in value)
                {
                    Serialize(oStream, item);
                }

                break;

            case object value:
                Serialize(oStream, obj);
                break;

            default:
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Serializes the provided object and returns its byte array representation.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>A byte array representing the serialized object.</returns>
    public byte[] Serialize(object obj)
    {
        using MemoryStream oStream = new();
        Serialize(oStream, obj);
        return oStream.ToArray();
    }

    /// <summary>
    /// Serializes the provided object and writes it to the provided stream.
    /// </summary>
    /// <param name="oStream">The stream to write the serialized object to.</param>
    /// <param name="obj">The object to serialize.</param>
    private void Serialize(MemoryStream oStream, object obj)
    {
        var oType = obj.GetType();
        if (obj is IOperation operation)
            WriteVarint32(oStream, (uint)operation.opid);
        
        if (oType.Namespace == "System")
            AddToStream(oStream, obj);
        else
        {
            var oFields = oType.Name == "KeyValuePair`2"
                ? (FieldInfo[])oType.GetTypeInfo().DeclaredFields
                : oType.GetFields();
            foreach (var oField in oFields)
            {
                if (oType.Name == "TransactionBody" && (oField.Name == "signatures" || oField.Name == "txid")) 
                    continue;
                
                if (oField.FieldType.Namespace == "System.Collections.Generic" && oField.GetValue(obj) == null)
                    WriteVarint32(oStream, 0);
                else
                {
                    if (Attribute.IsDefined(oField, typeof(OptionalField)))
                    {
                        if (oField.GetValue(obj) == null)
                        {
                            oStream.WriteByte(0);
                            continue;
                        }
                        else
                            oStream.WriteByte(1);
                    }

                    if (oField.GetValue(obj) == null)
                        throw new ArgumentNullException(oField.Name);
                    AddToStream(oStream, oField.GetValue(obj));
                }
            }
        }
    }
}