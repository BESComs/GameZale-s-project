using System;
using System.Collections.Generic;


[Serializable]
public class JsendWrap<T>
{
    public string status;
    public string message;
    public T data;
}