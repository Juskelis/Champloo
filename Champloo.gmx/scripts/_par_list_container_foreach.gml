/*
 * par_list_container per-component caller
 * It takes every component in the list and makes it
   call a script with parameters.
 * Used by container_foreach.
 */

var i, _size;

_size = ds_list_size(items);

for (i=0; i<_size; i++) {
    with (ds_list_find_value(items, i)) { script_execute(argument0, argument1); }
    }
