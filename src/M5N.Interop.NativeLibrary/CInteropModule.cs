﻿using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace M5N.Interop.NativeLibrary;

public sealed class CInteropModule : InteropModule
{
    private const string Target = "{5c4a08be-fafb-4952-b5a4-5a113bc36653}";

    public CInteropModule(string name)
    {
        System.Runtime.InteropServices.NativeLibrary.SetDllImportResolver(
            typeof(CInteropModule).Assembly,
            (search, _, _) =>
            {
                var handle = IntPtr.Zero;
                if (Target.Equals(search, StringComparison.Ordinal)) 
                    handle = System.Runtime.InteropServices.NativeLibrary.Load(name);
                return handle;
            });
    }

    [DllImport(Target)]
    public static extern void SetColour(byte colour);

    [DllImport(Target)]
    public static extern byte ChooseColour();

    [DllImport(Target)]
    public static extern void SetStone(byte x, byte y, byte colour);

    public static IEnumerable<object> PlaceStone()
    {
        unsafe
        {
            var p = PlaceStone_();
            return new object[] { p[0], p[1] };
            
            [DllImport(Target, EntryPoint = "PlaceStone")]
            static extern byte* PlaceStone_();
        }
    }

    [DllImport(Target)]
    public static extern ushort MakeDecision();

    [DllImport(Target)]
    public static extern void Victory();

    [DllImport(Target)]
    public static extern void Defeat();


    [DynamicDependency(nameof(SetColour))]
    [DynamicDependency(nameof(ChooseColour))]
    [DynamicDependency(nameof(SetStone))]
    [DynamicDependency(nameof(PlaceStone))]
    [DynamicDependency(nameof(MakeDecision))]
    [DynamicDependency(nameof(Victory))]
    [DynamicDependency(nameof(Defeat))]
    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        try
        {
            result = typeof(CInteropModule).InvokeMember(
                NameResolvingPolicy.Resolve(binder.Name),
                BindingFlags.Public | BindingFlags.Static, 
                null, 
                null, 
                args);
            return true;
        }
        catch (MissingMethodException)
        {
            result = null;
            return false;
        }
    }
}