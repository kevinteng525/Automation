require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
gem "minitest"
require "minitest/autorun"

#Main Matter items====================================find the matter,collection area,matter review,export and search folder,
#search,and so 
class Mattertreeitems < BrowserContainer
  # folder or search in matter review
  def search_in_folder(mattername,mattervalue,foldername,searchname)
    matteritems_mymatters(mattername,mattervalue).parent.div(:class=>'dijitTreeNodeContainer').div(:class=>'dijitTreeNode dijitTreeNodeLoaded dijitLoaded').div(:title=>foldername).parent.div(:title=>searchname)
  end
  
  
  # folder or search in matter review
  def folder_matterreview(mattername,mattervalue,foldername)
   if matter_review(mattername,mattervalue).exist?
     matter_review(mattername,mattervalue).parent.div(:class=>"dijitTreeNode dijitTreeNodeNotLoaded dijitNotLoaded dijitTreeIsLast").div(:title=>foldername)
   else
     matteritems_mymatters(mattername,mattervalue).parent.div(:class=>'dijitTreeNodeContainer').div(:class=>'dijitTreeNode dijitTreeNodeLoaded dijitLoaded').div(:title=>foldername)
   end
   #matteritems_mymatters(mattername,mattervalue).parent.div(:class=>'dijitTreeNodeContainer').div(:class=>'dijitTreeNode dijitTreeNodeLoaded dijitLoaded').div(:title=>foldername).wait_until_present
   #matteritems_mymatters(mattername,mattervalue).parent.div(:class=>'dijitTreeNodeContainer').div(:class=>'dijitTreeNode dijitTreeNodeLoaded dijitLoaded').div(:title=>foldername)
   #matter_review(mattername,mattervalue).div(:class=>'dijitTreeNodeContainer').div(:title=>foldername)
  end
  
  # folder or search '+' in matter review
  def folder_plus_matterreview(mattername,mattervalue,foldername)
    folder_matterreview(mattername,mattervalue,foldername).parent.span(:class=>'dijitInline dijitTreeExpando dijitTreeExpandoClosed')
  end
  
  # folder or search name in matter review
  def folder_name_matterreview(mattername,mattervalue,foldername)
    folder_matterreview(mattername,mattervalue,foldername).parent.span(:class=>'dijitTreeLabel matter_tree_label_enabled')
  end
  
   #folder or search context menu in matter review
   def folder_contextmenu_matterreview(mattername,mattervalue,foldername)
    folder_name_matterreview(mattername,mattervalue,foldername).click
    @browser.send_keys :tab
    @browser.send_keys :enter 
  end
  
  #all items in matter review
  def allitems(mattername,mattervalue,foldername)
    matter_review(mattername,mattervalue).div(:class=>'dijitTreeNodeContainer').div(:title=>'All Items').parent.span(:class=>'dijitTreeLabel matter_tree_label_enabled')
  end
  
  
  #start of search context menu
  def properties_search_contextmenu(mattername,mattervalue,foldername)
    folder_contextmenu(mattername,mattervalue,foldername)
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :enter
  end
    #start of search context menu
  def start_search_contextmenu(mattername,mattervalue,foldername)
    folder_contextmenu(mattername,mattervalue,foldername)
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :enter
  end
  
   #move of search context menu
  def move_search_contextmenu(mattername,mattervalue,foldername)
    folder_contextmenu(mattername,mattervalue,foldername)
    @browser.send_keys :down
    sleep(2)
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    sleep(2)
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :enter
  end
  
 #rename of search context menu
  def rename_search_contextmenu(mattername,mattervalue,foldername)
    folder_contextmenu(mattername,mattervalue,foldername)
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :enter
  end
  
  
  #delete of search context menu
  def delete_search_contextmenu(mattername,mattervalue,foldername)
    folder_contextmenu(mattername,mattervalue,foldername)
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :enter
  end
  
  #copy of search context menu
  def copy_search_contextmenu(mattername,mattervalue,foldername)
    folder_contextmenu(mattername,mattervalue,foldername)
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :enter
  end
  
  #newsearch of search context menu
  def newsearch_search_contextmenu(mattername,mattervalue,foldername)
    folder_contextmenu(mattername,mattervalue,foldername)
    @browser.send_keys :down
    @browser.send_keys :enter
  end
  
  #refresh of search context menu
  def refresh_search_contextmenu(mattername,mattervalue,foldername)
    folder_contextmenu(mattername,mattervalue,foldername)
    @browser.send_keys :enter
  end
  
  
   #folder or search context menu in collection
   def folder_contextmenu(mattername,mattervalue,foldername)
    folder_name(mattername,mattervalue,foldername).click
    @browser.send_keys :tab
    @browser.send_keys :enter
  end
  
   # folder or search '+' in collection area
  def folder_plus(mattername,mattervalue,foldername)
    folder_collectionarea(mattername,mattervalue,foldername).parent.span(:class=>'dijitInline dijitTreeExpando dijitTreeExpandoClosed')
  end
  
  # folder or search name in collection area
  def folder_name(mattername,mattervalue,foldername)
    folder_collectionarea(mattername,mattervalue,foldername).parent.span(:class=>'dijitTreeLabel matter_tree_label_enabled')
  end
  
  #verify the search is in the folder
  def verify_search_infolder(mattername,mattervalue,foldername,searchname)
    folder_name(mattername,mattervalue,foldername).click
    @browser.span(:text=>searchname).exist?
  end
  
  #verify the search is in the folder
  def verify_search_infolder_matterriview(mattername,mattervalue,foldername,searchname)
    folder_name_matterreview(mattername,mattervalue,foldername).click
    @browser.span(:text=>searchname).exist?
  end
  
# folder or search in collection area
  def folder_collectionarea(mattername,mattervalue,foldername)
   collection_area(mattername,mattervalue).div(:class=>'dijitTreeNodeContainer').div(:title=>foldername)
  end
  
  # get the '+' in export
  def export_plus(mattername,mattervalue)
    export(mattername,mattervalue).span(:class=>'dijitInline dijitTreeExpando dijitTreeExpandoClosed')#.span(:class=>'dijitTreeContent').span(:class=>'dijitTreeLabel matter_tree_label_enabled')
  end
  
  #get the context menu in export
  def export_contextmenu(mattername,mattervalue)
    export_name(mattername,mattervalue).click
    @browser.send_keys :tab
    @browser.send_keys :enter
  end
  #get the name in export
  def export_name(mattername,mattervalue)
    export(mattername,mattervalue).span(:class=>'dijitTreeContent')
  end
  
   #newfolder of matter context menu
  def newfolder_matterreview_contextmenu(mattername,mattervalue)
    matterreview_contextmenu(mattername,mattervalue)
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :enter
  end
  
  #newmatter of matter context menu
  def newsearch_matterreview_contextmenu(mattername,mattervalue)
    matterreview_contextmenu(mattername,mattervalue)
    @browser.send_keys :down
    sleep(10)
    @browser.send_keys :enter
    sleep(10)
    @browser.text_field(:id =>"new_search_name").set($searchname)
    newsearch = BtnIDHelper.new(@browser)
    newsearch.get_Dlg_btnID("New Search","OK")
  end
  
  #refresh of matter context menu
  def refresh_matterreview_contextmenu(mattername,mattervalue)
    matterreview_contextmenu(mattername,mattervalue)
    @browser.send_keys :enter
  end
  
  
  #get the context menu in matter review
  def matterreview_contextmenu(mattername,mattervalue)
    matterreview_name(mattername,mattervalue).click
    @browser.send_keys :tab
    @browser.send_keys :enter
  end
  # get the '+' in matter review
  def matterreview_plus(mattername,mattervalue)
    matter_review(mattername,mattervalue).span(:class=>'dijitInline dijitTreeExpando dijitTreeExpandoClosed')#.span(:class=>'dijitTreeContent').span(:class=>'dijitTreeLabel matter_tree_label_enabled')
  end
  #get the name in matter review
  def matterreview_name(mattername,mattervalue)
    matter_review(mattername,mattervalue).span(:class=>'dijitTreeContent')
  end
  
   #newfolder of matter context menu
  def newfolder_collectionarea_contextmenu(mattername,mattervalue)
    collectionarea_contextmenu(mattername,mattervalue)
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :enter
    @browser.text_field(:id =>"new_folder_name").set($foldername)
    newsearch = BtnIDHelper.new(@browser)
    newsearch.get_Dlg_btnID("New Folder","OK")
  end
  
  #newmatter of matter context menu
  def newsearch_collectionarea_contextmenu(mattername,mattervalue)
    collectionarea_contextmenu(mattername,mattervalue)
    @browser.send_keys :down
    @browser.send_keys :enter
    @browser.text_field(:id =>"new_search_name").set($searchname)
    newsearch = BtnIDHelper.new(@browser)
    newsearch.get_Dlg_btnID("New Search","OK")
 end
  
  #refresh of matter context menu
  def refresh_collectionarea_contextmenu(mattername,mattervalue)
    collectionarea_contextmenu(mattername,mattervalue)
    @browser.send_keys :enter
  end
 
  
  #get the context menu in collection are
   def collectionarea_contextmenu(mattername,mattervalue)
     collectionarea_name(mattername,mattervalue).click
     @browser.send_keys :tab
     @browser.send_keys :enter
  end
  
   # get the '+' in collection area
  def collectionarea_plus(mattername,mattervalue)
    collection_area(mattername,mattervalue).span(:class=>'dijitInline dijitTreeExpando dijitTreeExpandoClosed')#.span(:class=>'dijitTreeContent').span(:class=>'dijitTreeLabel matter_tree_label_enabled')
  end
  #get the name in collection are
  def collectionarea_name(mattername,mattervalue)
    collection_area(mattername,mattervalue).span(:class=>'dijitTreeContent')
  end
  
  # the collection area in matter by mattername
  def collection_area(mattername,mattervalue)
    matter_allproperties(mattername,mattervalue).div(:class=>'dijitTreeNodeContainer').divs(:class=>'dijitTreeNode dijitTreeNodeNotLoaded dijitNotLoaded')[0]
  end
  
  # the matter review in matter by mattername
  def matter_review(mattername,mattervalue)
    matter_allproperties(mattername,mattervalue).div(:class=>'dijitTreeNodeContainer').divs(:class=>'dijitTreeNode dijitTreeNodeNotLoaded dijitNotLoaded')[1]
  end
  
  # the export in matter by mattername
  def export(mattername,mattervalue)
    matter_allproperties(mattername,mattervalue).div(:class=>'dijitTreeNodeContainer').divs(:class=>'dijitTreeNode dijitTreeNodeNotLoaded dijitNotLoaded')[2]
  end
  
   #properties of matter context menu
  def properties_matter_contextmenu(mattername,mattervalue)
    matter_contextmenu(mattername,mattervalue)
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :enter
  end
  
  
  #remove from my matters of matter context menu
  def remove_matter_contextmenu(mattername,mattervalue)
    matter_contextmenu(mattername,mattervalue)
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :down
    @browser.send_keys :enter
  end
  
  #delete of matter context menu
  def delete_matter_contextmenu(mattername,mattervalue)
    matter_contextmenu(mattername,mattervalue)
    sleep(2)
    @browser.send_keys :down
    sleep(2)
    @browser.send_keys :down
    sleep(2)
    @browser.send_keys :enter
  end
  
  #newmatter of matter context menu
  def newmatter_matter_contextmenu(mattername,mattervalue)
    matter_contextmenu(mattername,mattervalue)
    @browser.send_keys :down
    @browser.send_keys :enter
  end
  
  #refresh of matter context menu
  def refresh_matter_contextmenu(mattername,mattervalue)
    matter_contextmenu(mattername,mattervalue)
    @browser.send_keys :enter
  end
  
  
  #get the matter context menu
  def matter_contextmenu(mattername,mattervalue)
    matter_click(mattername,mattervalue).click
    @browser.send_keys :tab
    @browser.send_keys :enter
  end
    
  #get the mattername ,u can user click method
  def matter_click(mattername,mattervalue)
    matter_allproperties(mattername,mattervalue).div(:class=>'dijitTreeRow').span(:class=>'dijitTreeContent').span(:class=>'dijitTreeLabel matter_tree_label_enabled')
  end
  
  
  #get the "+" in the left of mattername
  def matter_plus(mattername,mattervalue)
    matter_allproperties(mattername,mattervalue).span(:class=>'dijitInline dijitTreeExpando dijitTreeExpandoClosed')
  end
  
  #get the matter all properties by mattername
  def matter_allproperties(mattername,mattervalue)
    sleep(2)
    matteritems_mymatters(mattername,mattervalue).parent
  end
  
  #find the matter by mattername.mattervalue value is decide u select my matters or all matters
  #0->mymatters 1->allmatters
  def matteritems_mymatters(mattername,mattervalue)
    if mattervalue==0
    @browser.divs(:title => mattername)[0]
    else
    @browser.div(:id=>'allMattersTree').divs(:title => mattername)[0] 
    end
    
  end
  #find all matters item
  def all_matters_click
    @browser.span(:id=>'allMattersPane_button_title')
  end
  #matter drop down list context
  def dropdown_list_context
    @browser.div(:id=>"dijit_form_FilteringSelect_0_popup").divs(:class=>"dijitReset dijitMenuItem")
  end
  
  #matter drop down list context in all matter
  def dropdown_list_context_allmatter
    @browser.div(:id=>"dijit_form_FilteringSelect_1_popup").divs(:class=>"dijitReset dijitMenuItem")
  end
  
  #can find all matter from matter drop down list
  def dropdown_list
    @browser.div(:id=>"widget_dijit_form_FilteringSelect_0").div(:class=>"dijitReset dijitRight dijitButtonNode dijitArrowButton dijitDownArrowButton dijitArrowButtonContainer")
  end
  
  #can find all matter from matter drop down list in all matter
  def dropdown_list_allmatter
    @browser.div(:id=>"widget_dijit_form_FilteringSelect_1").div(:class=>"dijitReset dijitRight dijitButtonNode dijitArrowButton dijitDownArrowButton dijitArrowButtonContainer")
  end
  
  
end