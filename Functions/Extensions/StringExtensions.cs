namespace Functions.Extensions;

public static class StringExtensions
{
    public static string RemoveDodgyChars(this string buffer)
    {
        if (buffer.IndexOf('\u2013') > -1) buffer = buffer.Replace('\u2013', '-'); // en dash
        if (buffer.IndexOf('\u2014') > -1) buffer = buffer.Replace('\u2014', '-'); // em dash
        if (buffer.IndexOf('\u2015') > -1) buffer = buffer.Replace('\u2015', '-'); // horizontal bar
        if (buffer.IndexOf('\u2017') > -1) buffer = buffer.Replace('\u2017', '_'); // double low line
        if (buffer.IndexOf('\u2018') > -1) buffer = buffer.Replace('\u2018', '\''); // left single quotation mark
        if (buffer.IndexOf('\u2019') > -1) buffer = buffer.Replace('\u2019', '\''); // right single quotation mark
        if (buffer.IndexOf('\u201a') > -1) buffer = buffer.Replace('\u201a', ','); // single low-9 quotation mark
        if (buffer.IndexOf('\u201b') > -1) buffer = buffer.Replace('\u201b', '\''); // single high-reversed-9 quotation mark
        if (buffer.IndexOf('\u201c') > -1) buffer = buffer.Replace('\u201c', '\"'); // left double quotation mark
        if (buffer.IndexOf('\u201d') > -1) buffer = buffer.Replace('\u201d', '\"'); // right double quotation mark
        if (buffer.IndexOf('\u201e') > -1) buffer = buffer.Replace('\u201e', '\"'); // double low-9 quotation mark
        if (buffer.IndexOf('\u2026') > -1) buffer = buffer.Replace("\u2026", "..."); // horizontal ellipsis
        if (buffer.IndexOf('\u2032') > -1) buffer = buffer.Replace('\u2032', '\''); // prime
        if (buffer.IndexOf('\u2033') > -1) buffer = buffer.Replace('\u2033', '\"'); // double prime    }
        return buffer;
    }
}
