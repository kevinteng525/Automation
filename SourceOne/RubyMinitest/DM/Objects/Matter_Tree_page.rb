require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
gem "minitest"
require "minitest/autorun"

class MatterTreePage < BrowserContainer
  
  #get the matter tree nodes number.
  def getMatterTreeNode
    matter_tree.wait_until_present(60)
    matter_tree.attribute_value 'childElementCount'
  end
  
  #Expand the matter node.
  def expandMatterNode
    matter_node_closed.wait_until_present
    matter_node_closed.click
    @browser.wait
  end
  
  #Expand the matter node.
  def expandMatterReviewNode
    matterReview_node_closed.wait_until_present
    matterReview_node_closed.click
    @browser.wait
  end
  
  #Expand the matter node in All Matters.
  def expandMatterNodeAllMatters
    matter_node_closedOfAllMatters.wait_until_present
    matter_node_closedOfAllMatters.click
    @browser.wait
  end
  
  
 #select the All Matters
  def allMattersClick
    allMatters_node_closed.wait_until_present
    allMatters_node_closed.click
    @browser.wait
  end
  
  #Expand the Collection Area node.
  def expandCollectionAreaNode
    last_matter_collection_area_closed.wait_until_present
    last_matter_collection_area_closed.click
    @browser.wait
  end
  
  #Select the last Collection Area.
  def clickLastCollectionarea
    last_matter_collection_area.wait_until_present
    last_matter_collection_area.click
    @browser.wait(10)
  end
 
   #Select the last Matter Reviws in All matters.
  def clickLastMatterReviewAllMatters
    last_matter_Matter_ReviewAllMatters.wait_until_present
    last_matter_Matter_ReviewAllMatters.click
    @browser.wait(10)
  end
  
  #Select the last Matter Reviws.
  def clickLastMatterReview
    last_matter_Matter_Review.wait_until_present
    last_matter_Matter_Review.click
    @browser.wait(10)
  end
  
  
  #Verify the created matter is in the tree.
  def verifyMatterInTree(mname)
    @browser.div(:class=> 'dijitTreeRow' , :title => mname).exists?
  end
  
  #click on the created collapsed search node.
  def clickCollapsedSearchNode
    collapsed_search_node.wait_until_present
    collapsed_search_node.click
    @browser.wait(10)
  end
  
   #click on the created collapsed search node in Matter Review
  def clickCollapsedSearchNodeInMatterRev
    collapsed_search_node_inMatRew.wait_until_present
    collapsed_search_node_inMatRew.onclick()
    @browser.wait(10)
  end
  
   #click on the created collapsed Folder node
  def clickCollapsedFolderNode
    collapsed_Folder_node.wait_until_present
    collapsed_Folder_node.click
    @browser.wait(10)
  end
  
  #click on the search arrow.
  def clickSearchArrow
    search_arrow.wait_until_present
    search_arrow.click
    @browser.wait 
  end
  
  #Click on export node.
   def clickExportNodeExpanded
    export_node_expanded.wait_until_present
    export_node_expanded.click
    @browser.wait 
  end
  
  #click on context menu in matter tree.
  def clickMatterConextMenu
    matter_conextmenu.wait_until_present
    matter_conextmenu.click
    @browser.wait  
  end
  
  #click on a matter in tree.
  def clickMatterClosed
    matter_closed.wait_until_present
    matter_closed.click
    @browser.wait  
  end  
  
  #click on the "+" node of matter review.
  def clickMatterReviewPlusNode
    matterreview_plus_closed_node.wait_until_present
    matterreview_plus_closed_node.click
    @browser.wait
  end
  
  #click on the All Items node in the tree.
  def clickAllItemsNode
    allItems_node.wait_until_present
    allItems_node.click
    @browser.wait
  end
  
  
   def keyboardSearchToFolder
    sleep(2)
    @browser.send_keys :up
    @browser.send_keys :tab
    @browser.send_keys :enter
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :enter
   end
   
  
  private
  
  #The created matter's context menu in tree.
  def matter_conextmenu
    @browser.div(:title => $mattername)
  end
  
  #The All matters in tree.
  def allMatters_node_closed
    matter_tree.wait_until_present
    @browser.span(:id=>'allMattersPane_button_title')
  end
  
  #The created matter in tree.
  def matter_closed
    @browser.div(:title => $mattername).span(:class => 'dijitTreeContent').span(:class => 'dijitTreeLabel matter_tree_label_enabled')
  end
  
  
  #The created matter's "+" in tree in All matters.
  def matter_node_closedOfAllMatters
    @browser.div(:id=>'dijit__WidgetsInTemplateMixin_3').div(:title => $mattername).span(:class => 'dijitInline dijitTreeExpando dijitTreeExpandoClosed')
  end
  #The created matter's "+" in tree in My matters.
  def matter_node_closed
    @browser.div(:title => $mattername).span(:class => 'dijitInline dijitTreeExpando dijitTreeExpandoClosed')
  end
  
  
  
  #The last matter Collection Area in tree.(the node is closed.)
  def last_matter_collection_area
    matter_tree.wait_until_present
    @node = matter_tree.attribute_value 'childElementCount'
    @browser.div(:id => 'dijit__WidgetsInTemplateMixin_'+@node).div(:class => 'dijitTreeNodeContainer').div(:class => 'dijitTreeNode dijitTreeNodeNotLoaded dijitNotLoaded').div(:title => 'Collection Area').span(:class => 'dijitTreeContent')
  end
  
  #The last matter Matter Review in tree.(the node is closed.)
  def last_matter_Matter_ReviewAllMatters
    @browser.div(:id=>'dijit__WidgetsInTemplateMixin_5').div(:title => 'Matter Review').span(:class => 'dijitTreeContent')
  end
  
  #The last matter Matter Review in tree All Matters.(the node is closed.)
  def last_matter_Matter_Review
    matter_tree.wait_until_present
    @browser.div(:title => 'Matter Review').span(:class => 'dijitTreeContent')
  end
  
  #The last matter Collection Area '+' node in tree.(the node is closed.)
  def last_matter_collection_area_closed
    matter_tree.wait_until_present
    node = matter_tree.attribute_value 'childElementCount'
    @browser.div(:id => 'dijit__WidgetsInTemplateMixin_'+node).div(:class => 'dijitTreeNodeContainer').div(:class => 'dijitTreeNode dijitTreeNodeNotLoaded dijitNotLoaded').div(:title => 'Collection Area').span(:class => 'dijitInline dijitTreeExpando dijitTreeExpandoClosed')
  end
    
  #The collapsed created Search node.
  def collapsed_search_node
    @browser.div(:title => $searchname).span(:class => 'dijitTreeContent')
  end 
  
   #The collapsed created Search node in Matter Review
  def collapsed_search_node_inMatRew
    @browser.span(:title => $searchname)
  end 
  
  #The collapsed created Folder node.
  def collapsed_Folder_node
    @browser.span(:title => $foldername)
  end   
  
  #search arrow.
  def search_arrow
    @browser.span(:class => 'dijitReset dijitInline dijitArrowButtonInner')
  end
    
  #export node.
  def export_node_expanded
    @browser.div(:id => 'dijit__WidgetsInTemplateMixin_4').div(:class => 'dijitTreeRow').span(:class => 'dijitTreeContent dijitTreeContentExpanded')
  end
    
  #The matter pane.
  def matter_tree
    @browser.div(:class => 'dijitTreeNodeContainer')
  end
  
  #the Matter Review closed node in matter tree.
  def matterreview_closed_node
    @browser.div(:title => 'Matter Review').span(:class => 'dijitTreeContent').span(:class => 'dijitTreeLabel matter_tree_label_enabled')
  end  
  
  #the "+" of matter review closed node.
  def matterreview_plus_closed_node
    matter_tree.wait_until_present
    node = matter_tree.attribute_value 'childElementCount'
    @browser.div(:id => 'dijit__WidgetsInTemplateMixin_3').div(:class => 'dijitTreeRow').span(:class => 'dijitInline dijitTreeExpando dijitTreeExpandoClosed')
  end
  
  #All Items node in the matter tree.
  def allItems_node
    @browser.div(:title => 'All Items').span(:class => 'dijitTreeContent').span(:class => 'dijitTreeLabel matter_tree_label_enabled')
  end
end