/*
 * par_grid_container component adder ( [cellx,celly] )
 * When adding a component to grid, you can either
   provide cell coordinates in grid or don't give
   additional parameters.
 * When cell coordinates are provided, the component
   will be placed there, potentially removing other
   component occupying this place.
 * When no parameters are provided, the component
   will be placed in the first free field in grid,
   found by searching rows from top to down.
 */

//preparation
var _args;
var _x, _y;

_args = argument1;
_x = -1;
_y = -1;

//finding destination cell or use the ones provided

if (!ds_list_empty(_args)) {        //when destination cell is provided
    _x = ds_list_find_value(_args, 0);
    _y = ds_list_find_value(_args, 1);
    
    with (ds_grid_get(items, _x, _y)) { leave_container(); }    //kicking out other component, if any
    }
else {      //when destination cell must be found
    var i, j, _w, _h;
    
    _w = ds_grid_width(items);
    _h = ds_grid_height(items);    

    for (j=0; j<_h; j++) {
    for (i=0; i<_w; i++) {
        if (ds_grid_get(items, i, j) == noone) {
            _x = i; _y = j;
            break;
            }
        }
        
        if (_x != -1)       //break second loop if the field was already found
            break;
        }
    }

//adding the component to destination cell if provided or successfully found
if (_x != -1) {
    ds_grid_set(items, _x, _y, argument0);
    with (argument0) {
        x_in_container = _x;
        y_in_container = _y;
        }
    }