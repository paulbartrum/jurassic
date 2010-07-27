debugger;

var x = 0;
for (var i = 0, j = 0; i < 2; i++) {
    x += 5;
}

do
{
    x += 3;
} while (x < 15);

for (var y in { a: 1, b: 2 }) {
    x += 1;
}

var x;
var y;
var x = 15, y = 2, z;

while (x < 20) {
    if (x > 18) {
        break;
    }
    else {
        x++;
        continue;
    }
}

switch (x) {
    case 13:
        x = 0;
        break;
    default:
        x = 1;
        break;
}

try
{
    throw 5;
}
catch (e)
{
    x = 1;
}
finally
{
    x = 2;
}

try
{
    x = 3;
}
finally
{
    x = 4;
}

function f() {
    return 6;
}

with ({ a: 1 }) {
    f();
}