/*
 * obj_align_container constructor ( width,height )
 * Creates an align layout container, with given
   width and height of container region.
 */

var _args;

_args = argument0;

items = ds_grid_create( 3, 3 );
ds_grid_clear(items, noone);

if (ds_list_size(_args) < 2) {
    err("An align layout container must have region dimensions specified!");
    exit;
    }
_par_container_init(_args);