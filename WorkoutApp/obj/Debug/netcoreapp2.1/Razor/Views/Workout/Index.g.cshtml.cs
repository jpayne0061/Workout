#pragma checksum "C:\Users\Evan\source\repos\WorkoutApp\WorkoutApp\Views\Workout\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "e2f2025d5fc10dd0a81ee4fb62880b6bb5752bc6"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Workout_Index), @"mvc.1.0.view", @"/Views/Workout/Index.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Workout/Index.cshtml", typeof(AspNetCore.Views_Workout_Index))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\Users\Evan\source\repos\WorkoutApp\WorkoutApp\Views\_ViewImports.cshtml"
using WorkoutApp;

#line default
#line hidden
#line 2 "C:\Users\Evan\source\repos\WorkoutApp\WorkoutApp\Views\_ViewImports.cshtml"
using WorkoutApp.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e2f2025d5fc10dd0a81ee4fb62880b6bb5752bc6", @"/Views/Workout/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"87121c0692d67a1cf2ba59cb6ff7bc22bacce962", @"/Views/_ViewImports.cshtml")]
    public class Views_Workout_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<WorkoutApp.Models.Workout>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(47, 6, true);
            WriteLiteral("\r\n<h4>");
            EndContext();
            BeginContext(54, 58, false);
#line 3 "C:\Users\Evan\source\repos\WorkoutApp\WorkoutApp\Views\Workout\Index.cshtml"
Write(Html.ActionLink("Create New Workout", "Create", "Workout"));

#line default
#line hidden
            EndContext();
            BeginContext(112, 9, true);
            WriteLiteral("</h4>\r\n\r\n");
            EndContext();
#line 5 "C:\Users\Evan\source\repos\WorkoutApp\WorkoutApp\Views\Workout\Index.cshtml"
 foreach (var w in Model)
{

#line default
#line hidden
            BeginContext(151, 8, true);
            WriteLiteral("    <h2>");
            EndContext();
            BeginContext(160, 76, false);
#line 7 "C:\Users\Evan\source\repos\WorkoutApp\WorkoutApp\Views\Workout\Index.cshtml"
   Write(Html.ActionLink(w.WorkoutName, "Start", "Workout", new { id = w.WorkoutId }));

#line default
#line hidden
            EndContext();
            BeginContext(236, 7, true);
            WriteLiteral("</h2>\r\n");
            EndContext();
            BeginContext(247, 10, true);
            WriteLiteral("    <ul>\r\n");
            EndContext();
#line 11 "C:\Users\Evan\source\repos\WorkoutApp\WorkoutApp\Views\Workout\Index.cshtml"
         foreach (var exercise in w.WorkoutToExercise)
        {

#line default
#line hidden
            BeginContext(324, 16, true);
            WriteLiteral("            <li>");
            EndContext();
            BeginContext(341, 30, false);
#line 13 "C:\Users\Evan\source\repos\WorkoutApp\WorkoutApp\Views\Workout\Index.cshtml"
           Write(exercise.Exercise.ExerciseName);

#line default
#line hidden
            EndContext();
            BeginContext(371, 7, true);
            WriteLiteral("</li>\r\n");
            EndContext();
#line 14 "C:\Users\Evan\source\repos\WorkoutApp\WorkoutApp\Views\Workout\Index.cshtml"
        }

#line default
#line hidden
            BeginContext(389, 11, true);
            WriteLiteral("    </ul>\r\n");
            EndContext();
            BeginContext(402, 45, true);
            WriteLiteral("    <div style=\"margin-bottom: 15px\"></div>\r\n");
            EndContext();
#line 18 "C:\Users\Evan\source\repos\WorkoutApp\WorkoutApp\Views\Workout\Index.cshtml"
}

#line default
#line hidden
            BeginContext(450, 4, true);
            WriteLiteral("\r\n\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<WorkoutApp.Models.Workout>> Html { get; private set; }
    }
}
#pragma warning restore 1591
