/*
 * par_grid_container per-component caller
 * It takes every component in the grid and makes it
   call a script with parameters.
 * Used by container_foreach.
 */

var i, j;
var _w, _h;

_w = ds_grid_width(items);
_h = ds_grid_height(items);  

for (j=0; j<_h; j++) {
for (i=0; i<_w; i++) {
    with (ds_grid_get(items, i, j)) { script_execute(argument0, argument1); }
    }
    }