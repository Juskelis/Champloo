///lb_begin(container,arg1,arg2,...)
/*
 * Adds a new container to the currently built one,
   possibly using additional parameters such as e.g.
   grid cell coordinates.
 * Also, gives focus to the newly added container.
 */
 
var i, _params;

_params = ds_list_create();
for (i=1; i<argument_count; i++) {
    ds_list_add(_params, argument[i]);
    }

/****************
 * wrapped part *
 ****************/

//adding container to the current one (unless there's no container yet)
if (!ds_stack_empty(other.containers_stack))
    with (argument[0]) { _enter_container(ds_stack_top(other.containers_stack), _params); }

//giving focus to the newly created container
ds_stack_push(containers_stack, argument[0]);

/***********
 * endwrap *
 ***********/

ds_list_destroy(_params);
