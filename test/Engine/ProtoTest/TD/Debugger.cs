using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using ProtoCore.Lang;
using ProtoScript.Runners;
using ProtoCore.DSASM.Mirror;
using ProtoTestFx.TD;
using ProtoTestFx;
namespace ProtoTest.TD
{
    public class Debugger
    {
        readonly TestFrameWork thisTest = new TestFrameWork();
        ProtoCore.Core core;
        ProtoScript.Config.RunConfiguration runnerConfig;
        string testCasePath = @"..\..\..\Scripts\TD\Debugger\";
        ProtoScript.Runners.DebugRunner fsr;
        [SetUp]
        public void SetUp()
        {
            // Specify some of the requirements of IDE.
            ProtoCore.Options options = new ProtoCore.Options();
            options.ExecutionMode = ProtoCore.ExecutionMode.Serial;
            options.SuppressBuildOutput = false;
            core = new ProtoCore.Core(options);
            core.Executives.Add(ProtoCore.Language.kAssociative, new ProtoAssociative.Executive(core));
            core.Executives.Add(ProtoCore.Language.kImperative, new ProtoImperative.Executive(core));
            runnerConfig = new ProtoScript.Config.RunConfiguration();
            runnerConfig.IsParrallel = false;
            fsr = new ProtoScript.Runners.DebugRunner(core);
        }

        [Test]
        [Category("Debugger")]
        public void T001_SampleTest()
        {
            //string errorString = "1463735 - Sprint 20 : rev 2147 : breakpoint cannot be set on property ' setter' and 'getter' methods ";
            string src = string.Format("{0}{1}", testCasePath, "T001_SampleTest.ds");
            fsr.LoadAndPreStart(src, runnerConfig);
            ProtoCore.CodeModel.CodePoint cp = new ProtoCore.CodeModel.CodePoint
            {
                CharNo = 8,
                LineNo = 17,
                SourceLocation = new ProtoCore.CodeModel.CodeFile
                {
                    FilePath = Path.GetFullPath(src)
                }
            };
            fsr.ToggleBreakpoint(cp);
            // First step should land on line "p = Point.Point();".
            ProtoScript.Runners.DebugRunner.VMState vms = fsr.StepOver();
            Assert.AreEqual(14, vms.ExecutionCursor.StartInclusive.LineNo);
            // These are not used for now, so I'm commenting them out.
            // Object[] exp = { 1, 2, 3, new object[] { 4, 5 }, 6.0, 7, new object[] { 8.0, 9 } };
            // Object[] exp2 = new Object[] { exp, 10 };
            Obj stackValue = null;
            try
            {
                stackValue = vms.mirror.GetDebugValue("y");
            }
            catch (ProtoCore.DSASM.Mirror.UninitializedVariableException exception)
            {
                // Variable "y" isn't valid as of now.
                Assert.AreEqual("y", exception.Name);
            }
            vms = fsr.Run(); // Run to breakpoint on property setter "p.x = 20;".
            Assert.AreEqual(17, vms.ExecutionCursor.StartInclusive.LineNo);
            Assert.AreEqual(5, vms.ExecutionCursor.StartInclusive.CharNo);
            Assert.AreEqual(17, vms.ExecutionCursor.EndExclusive.LineNo);
            Assert.AreEqual(14, vms.ExecutionCursor.EndExclusive.CharNo);
            stackValue = vms.mirror.GetDebugValue("y");
            Assert.AreEqual("10", stackValue.Payload.ToString());
        }

        [Test]
        public void Defect_1467570_Crash_In_Debug_Mode()
        {
            string src = @" 
            fsr.PreStart(src, runnerConfig);
            DebugRunner.VMState vms = fsr.Step();   // myTest = Test.FirstApproach({ 1, 2 }); 
            ProtoCore.CodeModel.CodePoint cp = new ProtoCore.CodeModel.CodePoint
            {
                LineNo = 15,
                CharNo = 5
            };

            fsr.ToggleBreakpoint(cp);
            fsr.Run();  // line containing "this"            

            ExpressionInterpreterRunner watchRunner = new ExpressionInterpreterRunner(core);
            ExecutionMirror mirror = watchRunner.Execute(@"this");
            Obj objExecVal = mirror.GetWatchValue();
            Assert.AreNotEqual(null, objExecVal);
            Assert.AreNotEqual(null, objExecVal.Payload);
            Assert.AreEqual(mirror.GetType(objExecVal), "Test");
            vms = fsr.StepOver();

            watchRunner = new ExpressionInterpreterRunner(core);
            mirror = watchRunner.Execute(@"this");
            objExecVal = mirror.GetWatchValue();
            Assert.AreNotEqual(null, objExecVal);
            Assert.AreEqual(-1, (Int64)objExecVal.Payload);
            Assert.AreEqual(mirror.GetType(objExecVal), "null");
        }

        [Test]
        public void Defect_1467584_Crash_In_Debug_Mode()
        {
            string src = @" 
            DebugTestFx.CompareDebugAndRunResults(src);

        }

        [Test]
        public void Defect_1467584_Crash_In_Debug_Mode_2()
        {
            string src = @" 
            DebugTestFx.CompareDebugAndRunResults(src);
        }

        [Test]
        public void Defect_1467584_Crash_In_Debug_Mode_3()
        {
            string src = @"
            DebugTestFx.CompareDebugAndRunResults(src);
        }

        [Test]
        public void Defect_1467584_Crash_In_Debug_Mode_4()
        {
            string src = @" 
            DebugTestFx.CompareDebugAndRunResults(src);
        }


        [Test]
        public void Defect_1467584_Crash_In_Debug_Mode_5()
        {
            string src = @" 
            DebugTestFx.CompareDebugAndRunResults(src);
        }

        [Test]
        public void Defect_1467584_Crash_In_Debug_Mode_6()
        {
            string src = @" 
            DebugTestFx.CompareDebugAndRunResults(src);
        }

        [Test]
        public void Defect_1467584_Crash_In_Debug_Mode_7()
        {
            string src = @" 
            DebugTestFx.CompareDebugAndRunResults(src);
        }

        [Test]
        public void Defect_IDE_884_UsingBangInsideImperative_1()
        {
            string src = @" 
            DebugTestFx.CompareDebugAndRunResults(src);
        }

        [Test]
        public void Defect_IDE_884_UsingBangInsideImperative_2()
        {
            string src = @" 
            DebugTestFx.CompareDebugAndRunResults(src);
        }

        [Test]
        public void Defect_IDE_884_UsingBangInsideImperative_3()
        {
            string src = @" 
            DebugTestFx.CompareDebugAndRunResults(src);
        }

        [Test]
        [Category("WatchFx Tests")]
        public void T002_Defect_1467629_Debugging_InlineCondition_With_Multiple_Return_Types()
        {
            Dictionary<int, List<string>> map = new Dictionary<int, List<string>>();
            string src = @"class B
{
    b = 0;
    constructor B(xx)
    {
        b = xx;
    }
}
class A  
{
    x : var[]..[];
    constructor A(a : int) //example constructor 
    {
        x = a == 1 ? B.B(1) : { B.B(0), B.B(2) }; // using an in-line conditional
            
    }   
}
c = 2;
aa = A.A(c);
c = 1;";
            WatchTestFx fx = new WatchTestFx(); fx.CompareRunAndWatchResults(null, src, map);
        }

        [Test]
        [Ignore]
        [Category("WatchFx Tests")]
        public void T003_Defect_1467629_Debugging_InlineCondition_With_Multiple_Return_Types()
        {
            Dictionary<int, List<string>> map = new Dictionary<int, List<string>>();
            string src = @"import(""ProtoGeometry.dll"");
import(""Math.dll"");
class FixitySymbol // this class is implicitly extended from var 
{
    Origin : Point; // define the class properties.. 
    Size : double;
    IsFixed : bool;
    Symbol : var[]..[]; //defined 'by composition', by one or more Solids 
    constructor FromOriginSize(origin : Point, size : double, isFixed : bool) //example constructor 
    {
        Origin = origin; // by convention properties of the class (with uppercase names) 
        Size = size; // are populated from the corresponding arguments (with lowercase names) 
        IsFixed = isFixed;
        localWCS = CoordinateSystem.WCS;
        Symbol = isFixed ? // using an in-line conditional
            Cuboid.ByLengths(CoordinateSystem.ByOriginVectors(Origin, // if true 
            localWCS.XAxis, localWCS.YAxis), Size, Size, Size) : { Sphere.ByCenterPointRadius(Origin, Size * 0.25),
            Cone.ByCenterLineRadius(Line.ByStartPointDirectionLength(Origin, localWCS.ZAxis, -Size), Size * 0.01, Size * 0.5) };
    }
    def Move : FixitySymbol(x : double, y : double, z : double) // an instance method 
    {
        return = FixitySymbol.FromOriginSize(this.Origin.Translate(x, y, z), this.Size, this.IsFixed); // note: the use of 'this' key word to refer to the instance 
    }
    // in general instances of DesignScript class are immutable, and cannot be changed 
    // to give the illusion of change, this instance method actually calls a constructor 
    // and creates a new instance, but using some of the properties of the previous instance 
    def SetColor(color : Color)
    {
        this.Symbol = this.Symbol.SetColor(color); // call SetColor on constituent geometric properties 
        return = null;
    }
}
origin1 = Point.ByCoordinates(5, 5, 0); // define some appropriate input arguments 
origin2 = Point.ByCoordinates(0..10..5, 10, 0); // including a collection of points 
firstFixitySymbol = FixitySymbol.FromOriginSize(origin1, 2, true); // initially constructed 
firstFixitySymbol.SetColor(Color.Cyan); // set color 
firstFixitySymbol = firstFixitySymbol.Move(0, -4, 0); // modified by the instance method";
            WatchTestFx fx = new WatchTestFx(); fx.CompareRunAndWatchResults(null, src, map);
        }

        [Test]
        public void T004_Defect_IDE_963_Crash_On_Debugging()
        {
            string src = @" 
            DebugTestFx.CompareDebugAndRunResults(src);
        }

        [Test]
        public void T005_Defect_IDE_963_Crash_On_Debugging()
        {
            string src = @" 
            DebugTestFx.CompareDebugAndRunResults(src);
        }
    }
}