function GM_xmlhttpRequest(details) {
    if (!details)
        throw new Exception("details is undefined");

    if (!details.method)
        throw new Exception("details.method is required");

    if (!details.url)
        throw new Exception("details.url is required");

    uScriptHelper.xmlHttpRequest(details);
}