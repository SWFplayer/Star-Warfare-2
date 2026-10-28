using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Input Field")]
public class UIInput : MonoBehaviour
{
    public enum InputType { Standard = 0, AutoCorrect = 1, Password = 2 }
    public enum Validation { None = 0, Integer = 1, Float = 2, Alphanumeric = 3, Username = 4, Name = 5 }
    public enum KeyboardType { Default = 0, ASCIICapable = 1, NumbersAndPunctuation = 2, URL = 3, NumberPad = 4, PhonePad = 5, NamePhonePad = 6, EmailAddress = 7 }

    public delegate char OnValidate(string text, int charIndex, char addedChar);

    public static UIInput current;
    public static UIInput selection;

    public UILabel label;
    public InputType inputType;
    public KeyboardType keyboardType;
    public Validation validation;
    public int characterLimit;
    public string savedAs;
    public GameObject selectOnTab;
    public Color activeTextColor = Color.white;
    public List<EventDelegate> onSubmit = new List<EventDelegate>();
    public OnValidate onValidate;

    [SerializeField] protected string mValue;
    protected string mDefaultText = string.Empty;
    protected Color mDefaultColor = Color.white;
    protected float mPosition;
    protected bool mDoInit = true;
    protected UIWidget.Pivot mPivot;
    protected static int mDrawStart;
    protected static int mDrawEnd;
    protected static TouchScreenKeyboard mKeyboard;

    public string defaultText { get { return mDefaultText; } set { mDefaultText = value; } }

    [Obsolete("Use UIInput.value instead")]
    public string text { get { return value; } set { this.value = value; } }

    public string value
    {
        get
        {
            if (mDoInit) Init();
            if (isSelected && mKeyboard != null && mKeyboard.active) return mKeyboard.text;
            return mValue;
        }
        set
        {
            if (mDoInit) Init();
            if (isSelected && mKeyboard != null) mKeyboard.text = value;
            if (this.value != value)
            {
                mValue = value;
                if (isSelected && mKeyboard != null) mKeyboard.text = value;
                SaveToPlayerPrefs(mValue);
                UpdateLabel();
            }
        }
    }

    protected bool needsTextCursor { get { return isSelected && mKeyboard == null; } }

    [Obsolete("Use UIInput.isSelected instead")]
    public bool selected { get { return isSelected; } set { isSelected = value; } }

    public bool isSelected
    {
        get { return selection == this; }
        set
        {
            if (!value) { if (isSelected) UICamera.selectedObject = null; }
            else { UICamera.selectedObject = base.gameObject; }
        }
    }

    protected int cursorPosition { get { return value.Length; } }

    private void Start()
    {
        if (string.IsNullOrEmpty(mValue))
        {
            if (!string.IsNullOrEmpty(savedAs) && PlayerPrefs.HasKey(savedAs)) value = PlayerPrefs.GetString(savedAs);
        }
        else { value = mValue; }
    }

    protected void Init()
    {
        if (mDoInit && label != null)
        {
            mDoInit = false;
            mDefaultText = label.text;
            mDefaultColor = label.color;
            label.supportEncoding = false;
            mPivot = label.pivot;
            mPosition = label.cachedTransform.localPosition.x;
            UpdateLabel();
        }
    }

    protected void SaveToPlayerPrefs(string val)
    {
        if (!string.IsNullOrEmpty(savedAs))
        {
            if (string.IsNullOrEmpty(val)) PlayerPrefs.DeleteKey(savedAs);
            else PlayerPrefs.SetString(savedAs, val);
        }
    }

    protected virtual void OnSelect(bool isSelected)
    {
        if (isSelected) OnSelectEvent();
        else OnDeselectEvent();
    }

    protected void OnSelectEvent()
    {
        selection = this;
        if (mDoInit) Init();
        if (label != null && NGUITools.IsActive(this))
        {
            label.color = activeTextColor;

            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            {
                mKeyboard = (inputType != InputType.Password)
                    ? TouchScreenKeyboard.Open(mValue, (TouchScreenKeyboardType)keyboardType, inputType == InputType.AutoCorrect)
                    : TouchScreenKeyboard.Open(mValue, TouchScreenKeyboardType.Default, false, false, true);
            }
            else
            {
                
                mDrawStart = 0;
                mDrawEnd = 0;
            }
            UpdateLabel();
        }
    }

    protected void OnDeselectEvent()
    {
        if (mDoInit) Init();
        if (label != null && NGUITools.IsActive(this))
        {
            mValue = value;
            if (mKeyboard != null)
            {
                mKeyboard.active = false;
                mKeyboard = null;
            }
            if (string.IsNullOrEmpty(mValue))
            {
                label.text = mDefaultText;
                label.color = mDefaultColor;
            }
            else
            {
                label.text = mValue;
            }
            RestoreLabelPivot();
        }
        selection = null;
        UpdateLabel();
    }

    private void Update()
    {

        if (mKeyboard != null && isSelected && NGUITools.IsActive(this))
        {
            string text = mKeyboard.text;
            if (mValue != text)
            {
                ProcessInputText(text);
                UpdateLabel();
                if (mValue != text) mKeyboard.text = mValue;
            }
            if (mKeyboard.done)
            {
                if (!mKeyboard.wasCanceled) Submit();
                mKeyboard = null;
                isSelected = false;
            }
        }

        else if (isSelected && Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android)
        {
            HandlePCInput();
        }
    }

    private void HandlePCInput()
    {
        bool needsUpdate = false;
        string input = Input.inputString;

        if (!string.IsNullOrEmpty(input))
        {
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];


                if (c == '\b')
                {
                    if (mValue.Length > 0)
                    {
                        mValue = mValue.Substring(0, mValue.Length - 1);
                        needsUpdate = true;
                    }
                }

                else if (c == '\n' || c == '\r')
                {
                    Submit();
                    isSelected = false;
                    return;
                }

                else
                {
                    char validatedChar = c;
                    if (onValidate != null) validatedChar = onValidate(mValue, mValue.Length, c);
                    else if (validation != Validation.None) validatedChar = Validate(mValue, mValue.Length, c);

                    if (validatedChar != 0)
                    {
                        if (characterLimit <= 0 || mValue.Length < characterLimit)
                        {
                            mValue += validatedChar;
                            needsUpdate = true;
                        }
                    }
                }
            }
        }

        if (needsUpdate)
        {
            UpdateLabel();
        }
    }


    private void ProcessInputText(string newText)
    {
        mValue = string.Empty;
        for (int i = 0; i < newText.Length; i++)
        {

            if (newText[i] == '\n' || newText[i] == '\r') continue;

            char c = newText[i];
            if (onValidate != null) c = onValidate(mValue, mValue.Length, c);
            else if (validation != Validation.None) c = Validate(mValue, mValue.Length, c);

            if (c != 0) mValue += c;
        }

        if (characterLimit > 0 && mValue.Length > characterLimit)
        {
            mValue = mValue.Substring(0, characterLimit);
        }
    }

    protected void Submit()
    {
        if (NGUITools.IsActive(this))
        {
            current = this;
            mValue = value;
            EventDelegate.Execute(onSubmit);
            SaveToPlayerPrefs(mValue);
            current = null;
        }
    }

    protected void UpdateLabel()
    {

        if (!(label != null)) return;
        if (mDoInit) Init();

        bool flag = isSelected;
        string text = value;
        bool flag2 = string.IsNullOrEmpty(text);
        string text2;
        if (flag2)
        {
            text2 = ((!flag) ? mDefaultText : ((!needsTextCursor) ? string.Empty : "|"));
            RestoreLabelPivot();
        }
        else
        {
            if (inputType == InputType.Password)
            {
                text2 = string.Empty;
                int i = 0;
                for (int length = text.Length; i < length; i++) text2 += "*";
            }
            else text2 = text;

            int num = (flag ? Mathf.Min(text2.Length, cursorPosition) : 0);
            string text3 = text2.Substring(0, num);
            if (flag)
            {
                text3 += Input.compositionString;
                if (needsTextCursor) text3 += "|";
            }
            text2 = text3 + text2.Substring(num, text2.Length - num);

            if (label.overflowMethod == UILabel.Overflow.ClampContent)
            {
                if (flag)
                {
                    if (mDrawEnd == 0) mDrawEnd = num;
                    string text4 = text2.Substring(0, Mathf.Min(mDrawEnd, text2.Length));
                    int num2 = label.CalculateOffsetToFit(text4);
                    if (num < num2 || num >= mDrawEnd)
                    {
                        num2 = label.CalculateOffsetToFit(text3);
                        mDrawStart = num2;
                        mDrawEnd = text3.Length;
                    }
                    else if (num2 != mDrawStart) mDrawStart = num2;
                }
                if (mDrawStart != 0)
                {
                    text2 = text2.Substring(mDrawStart, text2.Length - mDrawStart);
                    if (mPivot == UIWidget.Pivot.Left) label.pivot = UIWidget.Pivot.Right;
                    else if (mPivot == UIWidget.Pivot.TopLeft) label.pivot = UIWidget.Pivot.TopRight;
                    else if (mPivot == UIWidget.Pivot.BottomLeft) label.pivot = UIWidget.Pivot.BottomRight;
                }
                else RestoreLabelPivot();
            }
            else RestoreLabelPivot();
        }
        label.text = text2;
        label.color = ((!flag2 || flag) ? activeTextColor : mDefaultColor);
    }

    protected void RestoreLabelPivot()
    {
        if (label != null && label.pivot != mPivot) label.pivot = mPivot;
    }

    protected char Validate(string text, int pos, char ch)
    {

        if (validation == Validation.None || !base.enabled) return ch;
        if (validation == Validation.Integer)
        {
            if (ch >= '0' && ch <= '9') return ch;
            if (ch == '-' && pos == 0 && !text.Contains("-")) return ch;
        }
        else if (validation == Validation.Float)
        {
            if (ch >= '0' && ch <= '9') return ch;
            if (ch == '-' && pos == 0 && !text.Contains("-")) return ch;
            if (ch == '.' && !text.Contains(".")) return ch;
        }
        else if (validation == Validation.Alphanumeric)
        {
            if (ch >= 'A' && ch <= 'Z') return ch;
            if (ch >= 'a' && ch <= 'z') return ch;
            if (ch >= '0' && ch <= '9') return ch;
        }
        else if (validation == Validation.Username)
        {
            if (ch >= 'A' && ch <= 'Z') return (char)(ch - 65 + 97);
            if (ch >= 'a' && ch <= 'z') return ch;
            if (ch >= '0' && ch <= '9') return ch;
        }
        else if (validation == Validation.Name)
        {
            char c = ((text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)]);
            char c2 = ((text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)]);
            if (ch >= 'a' && ch <= 'z')
            {
                if (c == ' ') return (char)(ch - 97 + 65);
                return ch;
            }
            if (ch >= 'A' && ch <= 'Z')
            {
                if (c != ' ' && c != '\'') return (char)(ch - 65 + 97);
                return ch;
            }
            switch (ch)
            {
                case '\'':
                    if (c != ' ' && c != '\'' && c2 != '\'' && !text.Contains("'")) return ch;
                    break;
                case ' ':
                    if (c != ' ' && c != '\'' && c2 != ' ' && c2 != '\'') return ch;
                    break;
            }
        }
        return '\0';
    }
}
