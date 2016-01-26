/*
    obj_fixed_grid_container constructor (width, height, num_cols, num_rows
    Creates a fixed dimension grid layout container, with given pixel
    width and height, and given number of cells wide and tall
*/

var _args = argument0;

if(ds_list_size(_args) < 4) {
    err("Fixed grid container needs all dimensions specified!");
    exit;
}

items = ds_grid_create(ds_list_find_value(_args,2), ds_list_find_value(_args,3));
ds_grid_clear(items, noone);

_par_container_init(_args);
