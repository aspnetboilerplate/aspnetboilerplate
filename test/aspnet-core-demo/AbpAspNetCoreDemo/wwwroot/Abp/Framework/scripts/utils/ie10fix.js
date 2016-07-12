//Windows Phone 8 and Internet Explorer 10 FIX
if (navigator.userAgent.match(/IEMobile\/10\.0/)) {
    var msViewportStyle = document.createElement("style");
    msViewportStyle.appendChild(
        document.createTextNode(
            "@-ms-viewport{width:auto!important}"
        )
    );
    
    document.getElementsByTagName("head")[0].appendChild(msViewportStyle);
}