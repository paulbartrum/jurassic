// JavaScript source code

function globalFuncion(arg1, arg2) {
    return arg1 + arg2;
}

function findMaximum(closureArg) {
    var maxVal = -Number.MAX_VALUE,
        maxIdx = -1,
        innerFunc = function (value, index) {
            if (value > maxVal) {
                maxVal = value;
                maxIdx = index;
            }
        }

    closureArg.forEach(innerFunc);
    return { value: maxVal, index: maxIdx };
}

function Constructor(data) {
    this.elements = data;
}

Constructor.prototype.findIndex = function (needle) {
    var idx = -1;
    this.elements.find(function (v, i) {
        if (!!v.match(needle)) {
            idx = i;
            return true;
        }
        else
            return false;
    });

    return idx;
}

globalFuncion(1, 3);

debugger;
var obj = new Constructor(["Julie", "John", "Juliet"]);
obj.findIndex(/Ju.+/);
