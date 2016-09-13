/*
 * par_container constructor ( [width,height] )
 * Creates a container with a given fixed width
   and height, if provided.
 * If no dimensions are provided, the container
   is assumed to adapt to the size of components.
 */
 
var _args;

_args = argument0;

if (!ds_list_empty(_args)) {
    width = ds_list_find_value(_args, 0);
    height = ds_list_find_value(_args, 1);
    adaptive = false;
    }
