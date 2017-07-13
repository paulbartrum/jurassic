/// Copyright (c) 2012 Ecma International.  All rights reserved. 
/// Ecma International makes this code available under the terms and conditions set
/// forth on http://hg.ecmascript.org/tests/test262/raw-file/tip/LICENSE (the 
/// "Use Terms").   Any redistribution of this code must retain the above 
/// copyright and this notice and otherwise comply with the Use Terms.

//Error Detector
if (this.window!==undefined) {  //for console support
    this.window.onerror = function(errorMsg, url, lineNumber) {
        this.window.iframeError = errorMsg;
    };
}

//This doesn't work with early errors in current versions of Opera
/*
if (/opera/i.test(navigator.userAgent)) {
    (function() {
        var origError = window.Error;
        window.Error = function() {
            if (arguments.length>0) {
                try {
                    window.onerror(arguments[0]);
                } catch(e) {
                    alert("Failed to invoke window.onerror (from ed.js)");
                }
            }
            return origError.apply(this, arguments);
        }
    })();
}*/