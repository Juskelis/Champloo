///container_arrange()
/*
 * Arranges all container items according
   to its layouting rule.
 * The rule is specified in a method "arrange"
   of the container.
 * It should be called whenever container
   is to be displayed after having its
   items change (e.g. adding, removal,
   resizing).
 */

if (!is(par_container) || !layout_changed)
    exit;

container_foreach(container_arrange);
    
if (layout_arranger != -1)
    script_execute(layout_arranger);
    
layout_changed = false;