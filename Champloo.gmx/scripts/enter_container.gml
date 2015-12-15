///enter_container(container,arg1,arg2,...)
/*
 * Registers calling component in a given container.
 * Might or might not use additional parameters.
 * How exactly the component is registrated depends on
   container's adding script, as well as parameters
   provided (if any).
 * The adding script is a method "add" of container.
 * Note that adding script must refer to
   the parameters not with argument[N],
   but with ds_list_find_value(argument0, N)!
 */

var i, _params;

_params = ds_list_create();
for (i=1; i<argument_count; i++) {
    ds_list_add(_params, argument[i]);
    }

/****************
 * wrapped part *
 ****************/

_enter_container(argument[0], _params);

/***********
 * endwrap *
 ***********/

ds_list_destroy(_params);