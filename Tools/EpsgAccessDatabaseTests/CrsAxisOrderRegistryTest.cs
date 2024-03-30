// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using BruTile.Wmts;
using NUnit.Framework;

namespace EpsgAccessDatabaseTests;

public class AxisOrderUtilityTest
{
    private const string EpsgAccessDatabase = @"D:\Daten\Epsg\EPSG_v8_9.mdb";

    private const string Sql =
        @"SELECT COORD_REF_SYS_CODE FROM [Coordinate Reference System] 
            WHERE COORD_SYS_CODE IN 
            ( SELECT CAA.COORD_SYS_CODE FROM [Coordinate Axis] AS CAA 
              INNER JOIN [Coordinate Axis] AS CAB 
              ON CAA.COORD_SYS_CODE = CAB.COORD_SYS_CODE 
              WHERE caa.ORDER=1 AND cab.ORDER=2
              AND ( 
               ( left(CAA.COORD_AXIS_ORIENTATION,5)='north' 
                 and left(CAB.COORD_AXIS_ORIENTATION,4)='east' ) 
               or ( left(CAA.COORD_AXIS_ORIENTATION,5)='south' 
                 and left(CAB.COORD_AXIS_ORIENTATION,4)='west' ) 
              ) 
            );";

    [Test, Explicit("This is not a test, it creates the bit field crs directions.\nYou need to have the EPSG Access Database at '" + EpsgAccessDatabase + "'!")]
    [Ignore("Use this one when finally resolving the axis order issue")]
    public void BuildAxisOrderBitArray()
    {
        var ba = new BitArray(32768);

        using (var cn = new System.Data.OleDb.OleDbConnection(
            $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={EpsgAccessDatabase};"))
        {
            cn.Open();
            var cmd = cn.CreateCommand();
            cmd.CommandText = Sql;
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                var code = dr.GetInt32(0);
                if (code > 32767) continue;
                ba[code] = true;
            }
        }

        string enc;
        var buffer = new byte[4096];
        ba.CopyTo(buffer, 0);

        using (var bufferStream = new MemoryStream(buffer))
        {
            using var compressedStream = new MemoryStream();
            using (var s = new DeflateStream(compressedStream, CompressionMode.Compress))
            {
                bufferStream.CopyTo(s);
                compressedStream.Flush();
            }
            enc = Convert.ToBase64String(compressedStream.ToArray());
            Console.WriteLine("Compressed");
            WriteBlocks(enc);
        }

        Console.WriteLine("\nByte array");
        WriteBytes(buffer, 20);

        enc = Convert.ToBase64String(buffer);
        Console.WriteLine("\nUncompressed");
        WriteBlocks(enc);

        Console.WriteLine("\nByte array");
        WriteBytes(buffer, 20);
    }

    private static void WriteBlocks(string text, int block = 60)
    {
        var i = 0;
        while (i + block < text.Length)
        {
            Console.WriteLine("\"{0}\" + ", text.Substring(i, block));
            i += block;
        }
        Console.WriteLine("\"{0}\"", text[i..]);
    }

    private static void WriteBytes(byte[] buffer, int bytesPerRow)
    {
        Console.Write("raw bytes\n{ ");

        var i = 0;
        byte[] tmp;
        while (i + bytesPerRow < buffer.Length)
        {
            tmp = new byte[bytesPerRow];
            Buffer.BlockCopy(buffer, i, tmp, 0, bytesPerRow);
            Console.WriteLine(WriteBytesRow(tmp));
            i += bytesPerRow;
        }

        bytesPerRow = buffer.Length - i;
        tmp = new byte[bytesPerRow];
        Buffer.BlockCopy(buffer, i, tmp, 0, bytesPerRow);
        var txt = WriteBytesRow(tmp);
        txt = txt[0..^1];

        Console.WriteLine(txt + "};");
    }

    private static string WriteBytesRow(byte[] buffer)
    {
        var sb = new StringBuilder();
        foreach (var b in buffer)
            sb.AppendFormat(" {0},", b);
        return sb.ToString();
    }

    [Test]
    [Ignore("Use this one when finally resolving the axis order issue")]
    public void TestAxisOrder()
    {
        if (!File.Exists(EpsgAccessDatabase))
            throw new IgnoreException("Epsg Access Database not found");

        var unusual = new HashSet<int>();

        using (var cn = new System.Data.OleDb.OleDbConnection(
            $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={EpsgAccessDatabase};"))
        {
            cn.Open();
            var cmd = cn.CreateCommand();
            cmd.CommandText = Sql;
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                var code = dr.GetInt32(0);
                if (code > 32767) continue;
                unusual.Add(code);
            }
        }
        var crsAxisOrderRegistry = new CrsAxisOrderRegistry();

        for (var code = 1; code < 32768; code++)
            if (CrsIdentifier.TryParse("urn:ogc:def:crs:EPSG::" + code, out var crs))
            {
                var expected = unusual.Contains(code) ? 1 : 0;
                Assert.That(crsAxisOrderRegistry[crs][0], Is.EqualTo(expected));
            }
    }
}
