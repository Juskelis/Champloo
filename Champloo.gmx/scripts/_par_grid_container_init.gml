/*
 * par_grid_container constructor ( gwidth,gheight, [cwidth,cheight] )
 * Creates a grid container with a given grid
   width and height, and optionally container
   region width and height, if provided.
 */

var _args;

_args = argument0;

if (ds_list_size(_args) < 2) {
    err("A grid container constructor must have grid width and height provided!");
    exit;
    }
items = ds_grid_create( ds_list_find_value(_args, 0), ds_list_find_value(_args, 1) );
ds_grid_clear(items, noone);

//removes the grid width and height parameters
//and then passes what remains to main container constructor
ds_list_delete(_args, 0);
ds_list_delete(_args, 0);
_par_container_init(_args);
