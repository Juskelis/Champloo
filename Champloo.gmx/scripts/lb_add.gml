///lb_add(component,arg1,arg2,...)
/*
 * Adds a new component to the currently built container,
   possibly using additional parameters like e.g.
   grid cell coordinates.
 */

var i, _params;

_params = ds_list_create();
for (i=1; i<argument_count; i++) {
    ds_list_add(_params, argument[i]);
    }

/****************
 * wrapped part *
 ****************/

with (argument[0]) { _enter_container(ds_stack_top(other.containers_stack), _params); }

/***********
 * endwrap *
 ***********/

ds_list_destroy(_params);