using System;
using System.Collections;
using System.Collections.Generic;

using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public abstract class WWWRequest
{
    public WWWBehaviorScript Behavior { get; set; }

    public IEnumerator Send()
    {
        WWW www = null;
        if (this.GetBytes() == null)
        {
            www = new WWW(this.GetUrl());
        }
        else
        {
            Hashtable _htable = new Hashtable();
            _htable.Add("Connection", "close");
            www = new WWW(this.GetUrl(), this.GetBytes(), _htable);
        }
        if (null != DebugUI.mInstance)
        {
            DebugUI.mInstance.SetProtocol(www.url);
        }
        DateTime startTime = DateTime.Now;
        bool timeout = false;
        while (!www.isDone)
        {
            if ((DateTime.Now - startTime).TotalSeconds > 15.0)
            {
                timeout = true;
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }
        try
        {
            if (timeout)
            {
                Debug.Log("WWW time out: " + this);
                this.OnTimeOut();
            }
            else if (www == null)
            {
                Debug.Log("WWW is null: " + this);
                this.OnResponseError();
            }
            else if (www.error != null)
            {
                Debug.Log(string.Concat(new object[]
                {
                "www.error: ",
                www.error,
                ";  ",
                this
                }));
                this.OnResponseError();
            }
            else if (www.bytes != null && www.bytes.Length > 0)
            {
                byte[] indexBytes = www.bytes;
                BytesBuffer bb = new BytesBuffer(indexBytes);
                this.ReadData(bb);
                GameApp.GetInstance().GetWWWManager().getReceivedPacketCache().AddPacket(this);
            }
            else
            {
                Debug.Log("www.bytes is null or www.bytes.Length <= 0: " + this);
                this.OnResponseError();
            }
        }
        catch (Exception ex2)
        {
            Exception ex = ex2;
            Debug.Log(this + "\n" + ex.Message);
            this.OnResponseError();
        }
        finally
        {
            if (www != null)
            {
                www.Dispose();
                www = null;
            }
        }
        yield break;
    }

    protected virtual BytesBuffer GetBody()
    {
        return null;
    }

    private byte[] GetBytes()
    {
        BytesBuffer body = this.GetBody();
        if (body != null)
        {
            return body.GetValidBytes();
        }
        return null;
    }

    protected string ToBase64String(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String(bytes);
    }

    protected string Hash(string toHash)
    {
        MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
        string s = toHash + "T83x4Pj03o";
        byte[] array = Encoding.ASCII.GetBytes(s);
        array = md5CryptoServiceProvider.ComputeHash(array);
        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte b in array)
        {
            stringBuilder.AppendFormat("{0:x2}", b);
        }
        return stringBuilder.ToString();
    }

    protected virtual string GetUrl()
    {
        return string.Empty;
    }

    protected virtual void ReadData(BytesBuffer bb)
    {
    }

    public virtual void ProcessLogic()
    {
    }

    public virtual void OnTimeOut()
    {
    }

    public virtual void OnResponseError()
    {
    }

    public virtual void OnVerifyResponseError()
    {
    }

    public virtual void OnVerifyFail(int errorCode)
    {
    }

    protected const float MAX_WAITING_TIME = 15f;

    protected const string MD5_KEY = "T83x4Pj03o";

    protected int mReceiveErrorCode;

    protected string mSign = string.Empty;

    protected bool mHasVerified;
}
