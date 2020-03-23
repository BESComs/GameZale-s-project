///
///  @file SROptions.Live.cs
///  @brief 機能説明
///
///  Copyright (c) 2016- R-FORCE ENTERTAINMENT inc. All rights reserved.
///  @author 所有者名
///
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SRDebugger;
using SRDebugger.Services;
using SRF;
using SRF.Service;

using System;
using System.ComponentModel;
using System.Diagnostics;

public class SROptions2
{
    private float test;

    public enum DebugMenuMain
    {
        InGame,
        OutGame,
        System
    }

    private DebugMenuMain debugMenuMain;

    [Category("Main")]
    public DebugMenuMain Main
    {
        get { return debugMenuMain; }
        set
        {
            //OnValueChanged("TestEnum", value);
            debugMenuMain = value;
        }
    }

    [Category("Test")]
    public float TestFloat2
    {
        get { return test; }
        set
        {
            test = value;
        }
    }

}

public class SROptions3
{
    private float test;

    public enum DebugMenuMain
    {
        InGame,
        OutGame,
        System
    }

    private DebugMenuMain debugMenuMain;

    [Category("Main2")]
    public DebugMenuMain Main
    {
        get { return debugMenuMain; }
        set
        {
            //OnValueChanged("TestEnum", value);
            debugMenuMain = value;
        }
    }

    [Category("Test2")]
    public float TestFloat2
    {
        get { return test; }
        set
        {
            test = value;
        }
    }

}
