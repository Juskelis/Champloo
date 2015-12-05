///container_add(container,component,arg1,arg2,...)
/*
 * Registers a component in a given container.
   Might or might not use additional parameters.
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
for (i=2; i<argument_count; i++) {
    ds_list_add(_params, argument[i]);
    }

/****************
 * wrapped part *
 ****************/

with (argument[1]) { _enter_container(argument[0], _params); }

/***********
 * endwrap *
 ***********/

ds_list_destroy(_params);