using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EnumAttribute : Attribute
{
    private string m_strDescription;
    public EnumAttribute(string strPrinterName)
    {
        m_strDescription = strPrinterName;
    }

    public string Description
    {
        get { return m_strDescription; }
    }
}