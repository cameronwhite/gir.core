﻿using static Bullseye.Targets;
using static DotNet;
using static Projects;
using System.Collections.Generic;
using System.IO;

class Program
{
    private static string configuration = confDebug;
    private const string confRelease = "Release";
    private const string confDebug = "Debug";

    private static readonly string[] sampleProjects =
    {
        DBUS_SAMPLE,
        GST_SAMPLE,
        GTK3_APP_SAMPLE,
        GTK3_MINIMAL_SAMPLE,
        GTK4_SIMPLE_WINDOW_SAMPLE
    };
    
    private static readonly string[] libraryProjects = 
    {
        GLIB_WRAPPER,
        CAIRO_WRAPPER,
        XLIB_WRAPPER,
        PANGO_WRAPPER,
        GDK3_WRAPPER,
        GIO_WRAPPER,
        GOBJECT_WRAPPER,
        GDK_PIXBUF_WRAPPER,
        GTK3_WRAPPER,
        WEBKIT2WEBEXTENSION_WRAPPER,
        CLUTTER_WRAPPER,
        GTKCLUTTER_WRAPPER,
        CHAMPLAIN_WRAPPER,
        GTKCHAMPLAIN_WRAPPER,
        GST_WRAPPER,
        GDK4_WRAPPER, //GTK4
        GSK4_WRAPPER, //GTK4
        GTK4_WRAPPER, //GTK4
        GOBJECT_CORE,
        GDK_PIXBUF_CORE,
        GLIB_CORE,
        GIO_CORE,
        GTK3_CORE,
        GDK4_CORE,
        GTK4_CORE,
        HANDY_CORE,
        WEBKITGTK_CORE,
        JAVASCRIPT_CORE_CORE,
        WEBKIT2WEBEXTENSION_CORE,
        CLUTTER_CORE,
        GTKCLUTTER_CORE,
        CHAMPLAIN_CORE,
        GTKCHAMPLAIN_CORE,
        GST_CORE
    };

    static void Main(string[] args)
    {        
        Target<(string project, string girFile, string import, bool addAlias)>(Targets.GenerateWrapper, 
            ForEach(
                (GLIB_WRAPPER, "GLib-2.0.gir", "libglib-2.0.so.0", false),
                (GOBJECT_WRAPPER, "GObject-2.0.gir", "libgobject-2.0.so.0", true),
                (GIO_WRAPPER, GIO_GIR, "libgio-2.0.so.0", true),
                (CAIRO_WRAPPER, "cairo-1.0.gir", "TODO", false),
                (XLIB_WRAPPER, "xlib-2.0.gir", "TODO", false),
                (PANGO_WRAPPER, "Pango-1.0.gir", "TODO", false),
                (GDK3_WRAPPER, "Gdk-3.0.gir", "TODO", true),
                (GDK_PIXBUF_WRAPPER, GDK_PIXBUF_GIR, "libgdk_pixbuf-2.0.so.0", true),
                (GTK3_WRAPPER, GTK3_GIR, "libgtk-3.so.0", true),
                (JAVASCRIPT_CORE_WRAPPER, JAVASCRIPT_CORE_GIR, "javascriptcoregtk-4.0.so", false),
                (HANDY_WRAPPER, HANDY_GIR, "libhandy-0.0.so.0", false),
                (WEBKITGTK_WRAPPER, WEBKITGTK_GIR, "libwebkit2gtk-4.0.so.37", true),
                (WEBKIT2WEBEXTENSION_WRAPPER, WEBKIT2WEBEXTENSION_GIR, "WEBEXTENSION", true),
                (CLUTTER_WRAPPER, "Clutter-1.0.gir", "libclutter-1.0.so.0", false),
                (GTKCLUTTER_WRAPPER, "GtkClutter-1.0.gir", "libclutter-gtk-1.0.so.0", false),
                (CHAMPLAIN_WRAPPER, "Champlain-0.12.gir", "libchamplain-0.12", false),
                (GTKCHAMPLAIN_WRAPPER, "GtkChamplain-0.12.gir", "libchamplain-gtk-0.12.so.0", false),
                (GST_WRAPPER, "Gst-1.0.gir", "libgstreamer-1.0.so.0", true),
                (GDK4_WRAPPER, "Gdk-4.0.gir", "libgtk-4.so.0", true),//GTK4
                (GSK4_WRAPPER, "Gsk-4.0.gir", "libgtk-4.so.0", true),//GTK4
                (GTK4_WRAPPER, GTK4_GIR, "libgtk-4.so.0", true) //GTK4
                ),
            (x) => GenerateWrapper(x.project, x.girFile, x.import, x.addAlias)
        );

        Target<(string project, string girFile)>(Targets.GenerateCore,
            ForEach(
                (GTK3_CORE, GTK3_GIR),
                (GDK_PIXBUF_CORE, GDK_PIXBUF_GIR),
                (GIO_CORE, GIO_GIR),
                (GTK4_CORE, GTK4_GIR),
                (HANDY_CORE, HANDY_GIR),
                (JAVASCRIPT_CORE_CORE, JAVASCRIPT_CORE_GIR),
                (WEBKIT2WEBEXTENSION_CORE, WEBKIT2WEBEXTENSION_GIR),
                (WEBKITGTK_CORE, WEBKITGTK_GIR)
            ),
            (x) => GenerateCore(x.project, x.girFile)
        );

        Target(Targets.Build, DependsOn(Targets.Generate),
            ForEach(libraryProjects),
            (project) => Build(project, configuration)
        );

        Target(Targets.CleanLibs, 
            ForEach(libraryProjects), 
            (project) => CleanUp(project, configuration)
        );
        
        Target(Targets.CleanSamples, 
            ForEach(sampleProjects), 
            (project) => CleanUp(project, configuration)
        );
        
        Target(Targets.Generate, DependsOn(Targets.GenerateWrapper, Targets.GenerateCore));
        Target(Targets.Clean, DependsOn(Targets.CleanLibs, Targets.CleanSamples));
        
        Target(Targets.Samples, DependsOn(Targets.Build), 
            ForEach(sampleProjects),
            (project) => Build(project, configuration)
        );
        
        Target(Targets.Release, () => configuration = confRelease);
        Target(Targets.Debug, () => configuration = confDebug);

        Target("default", DependsOn(Targets.Build));
        RunTargetsAndExit(args);
    }

    private static void CleanUp(string project, string configuration)
    {
        const string? generatedDir = "Generated";
        foreach (var dictionary in Directory.EnumerateDirectories(project))
            if(dictionary == generatedDir)
                Directory.Delete(Path.Combine(project, generatedDir));

        var classFolder = Path.Combine(project, "Classes");
        if(Directory.Exists(classFolder))
            foreach(var file in Directory.EnumerateFiles(classFolder))
                if (file.Contains(".Generated."))
                    File.Delete(file);
            
        Clean(project, configuration);
    }

    private static void GenerateCore(string project, string girFile)
    {
        girFile = $"../gir-files/{girFile}";
        var g = new Generator.CoreGenerator(girFile, Path.Combine(project, "Classes"));
        g.Generate();
    }
    
    private static void GenerateWrapper(string project, string girFile, string import, bool addGlibAliases)
    {
        girFile = $"../gir-files/{girFile}";
        var outputDir = Path.Combine(project,"Generated");

        var list = new List<string>();
        if(addGlibAliases)
            list.Add("../gir-files/GLib-2.0.gir");

        var g = new Generator.WrapperGenerator(girFile, outputDir, import, list);
        g.Generate();
    }
}