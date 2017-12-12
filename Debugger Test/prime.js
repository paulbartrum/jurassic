// JavaScript source code

// Break into the debugger. Make sure "Just My Code" is disabled to enable single stepping.
debugger;

function globalFunction(arg1, arg2) {
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

console.log("Adding (1 + 3) w/ `globalFunction`: " + globalFunction(1, 3));

var stat = findMaximum([5, 2, 6, 8, 3, 5, 0, 1]);
console.log("Finding maximum from `[5, 2, 6, 8, 3, 5, 0, 1]`: " + stat.value + " @ " + stat.index);

var obj = new Constructor(["Julie", "John", "Juliet"]);
console.log("Finding index of `/Jo.+/` in ['Julie', 'John', 'Juliet']: " + obj.findIndex(/Jo.+/));

function isPrime(num) {
    output = true
    for (var i = 2; i < num; i++) {
        if (num % i === 0) {
            output = false;
            break;
        }
    }
    return output
}

function findNthPrime(num) {
    var count = 0;
    for (var i = 2; i < 10000; i++) {
        if (isPrime(i) === true) {
            count = count + 1;
        }
        if (count === num) {
            return i;
        }
    }

}

// Print the 10th prime.
console.log("The %ith prime is %i.", 10, findNthPrime(10));