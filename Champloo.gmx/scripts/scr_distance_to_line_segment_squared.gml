/*
    calculates distance point p is from line defined by ab
    0 - a.x
    1 - a.y
    2 - b.x
    3 - b.y
    4 - p.x
    5 - p.y
*/

var ax = argument0;
var ay = argument1;
var bx = argument2;
var by = argument3;

var px = argument4;
var py = argument5;


var squarelength =  sqr(bx - ax) + sqr(by - ay);

if(squarelength == 0) return sqr(px - ax) + sqr(py - ay);

//p - a, b - a
var t = dot_product(px - ax, py - ay, bx - ax, by - ay)/squarelength;
if(t < 0) return sqr(px - ax) + sqr(py - ay);
if(t > 1) return sqr(px - bx) + sqr(py - by);

var projx = ax + t * (bx - ax);
var projy = ay + t * (by - ay);

return sqr(px - projx) + sqr(py - projy);
