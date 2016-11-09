require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
require_relative '..\Objects\Login_page.rb'
require_relative '..\Objects\Main_Matter_page.rb'
require_relative '..\Objects\New_Matter_page.rb'
require_relative '..\Objects\Matter_Tree_page.rb'
require_relative '..\Objects\New_Search_page.rb'
require_relative '..\Objects\DM_Main_Head_page.rb'
require_relative '..\Objects\CollectionArea_page.rb'
require_relative '..\Objects\Search_Result_page.rb'
require_relative '..\Objects\Export_Setting_page.rb'
require_relative '..\Objects\Export_Summary_page.rb'
require_relative '..\Objects\Main_Identity_page.rb'
require_relative '..\Objects\New_Identity_page.rb'
require_relative '..\Objects\Delete_Identity_page.rb'
require_relative '..\Objects\Delete_Matter_page.rb'
require_relative '..\Objects\Main_Administration_page.rb'
require_relative '..\Objects\Find_User_List_page.rb'
require_relative '..\Objects\Select_User_Add_page.rb'
require_relative '..\Objects\Delete_User_page.rb'
require_relative '..\Objects\All_Items_page.rb'
require_relative '..\Objects\Tag_Manger.rb'
require_relative '..\Objects\Matter_Properties_Dialog.rb'
require_relative '..\Objects\Matter_tree_items.rb'
gem "minitest"
require 'minitest/autorun'
require_relative '..\Objects\minitest_saber_base.rb'
require_relative '..\Objects\saber_base.rb'
#Smoke test cases.
class TestSuite < SaberTestBase
  include SaberBase
  
  def setup
    #ingest_test_data_to_souceone
    @dmweb = Site.new(Watir::Browser.new)
  end
  
  def teardown
    
    #delete the matter created
    @dmweb.close
    @dmweb1 = Site.new(Watir::Browser.new)
    @dmweb1.login_page.open($DMWebSite)
    @dmweb1.login_page.login_as($username, $password)
    @dmweb1.main_head_page.clickMatterLink
    @dmweb1.Mattertree_items.delete_matter_contextmenu($mattername,0)
    @dmweb1.btn_id_helper.get_Dlg_btnID("Confirmation","OK")
    @dmweb1.close
  end
  
  #Make the test case run by order. 
  def self.test_order
    :alpha
  end    
  
  
 
   #Test case #4: can't delete tag which assigned to items.
  def test_001_delete_tag_webid_677
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    @dmweb.main_matter_page.new_matter_click
    @dmweb.new_matter_page.set_MatterName($mattername)
    @dmweb.new_matter_page.click_OK
    @dmweb.main_matter_page.matter_properties_click
    sleep(2)
    @dmweb.btn_id_helper.get_Dlg_btnID("#{$mattername} - Matter Properties","Search Folders")
    @dmweb.Matter_Properties_Dialog.click_searchfolder_bt("#{$mattername} - Matter Properties",["25EmbeddedFoldersMappedFolder"])
    @dmweb.btn_id_helper.get_Dlg_btnID("#{$mattername} - Matter Properties","OK")
    @dmweb.main_head_page.clickMatterLink
    sleep(5)
    @dmweb.matter_tree_page.expandMatterNode
    @dmweb.matter_tree_page.clickLastCollectionarea
    @dmweb.main_matter_page.new_search_click
    @dmweb.new_search_page.set_SearchName($searchname)
    @dmweb.new_search_page.searchclick_OK
    @dmweb.search_result_page.clickSearchFindButton
    sleep(60)
    @dmweb.search_result_page.checkAllResult
    @dmweb.search_result_page.clickAssign("Assign")
    @dmweb.search_result_page.enterAssignToMatter
    sleep(5)
    @dmweb.matter_tree_page.clickMatterReviewPlusNode
    @dmweb.matter_tree_page.clickAllItemsNode
    @dmweb.search_result_page.checkAllResult
    sleep(2)
    @dmweb.allItems_result_page.clickAssign("Tags")
    @dmweb.allItems_result_page.enterAssignToTags
    tagname=@dmweb.allItems_result_page.get_tag
    @dmweb.main_head_page.clickMatterLink
    @dmweb.matter_tree_page.clickMatterClosed
    @dmweb.main_matter_page.matter_properties_click
    sleep(2)
    @dmweb.btn_id_helper.get_Dlg_btnID("test_gff - Matter Properties","Tags")
    @dmweb.Tag_Manger.focusToTags
    @dmweb.Tag_Manger.clickDefaultTag(tagname)
    @dmweb.Tag_Manger.clickDeleteButton1
    @dmweb.btn_id_helper.get_Dlg_btnID("Confirmation","Yes")
    @dmweb.btn_id_helper.get_Dlg_btnID("test_gff - Matter Properties","OK")
    @dmweb.btn_id_helper.get_Dlg_btnID("Error","OK")
    @dmweb.main_head_page.clickMatterLink
    @dmweb.matter_tree_page.clickMatterClosed
    @dmweb.main_matter_page.matter_properties_click
    sleep(2)
    @dmweb.btn_id_helper.get_Dlg_btnID("test_gff - Matter Properties","Tags")
    if @dmweb.Tag_Manger.click__Default_Tag(tagname).exist?
      puts "can't delete tag which assigned to items"
      assert(true,message="can't delete tag which assigned to items")
    else  
      puts "delete tag which assigned to items,maybe have a problem"  
      assert(false,message="delete tag which assigned to items,maybe have a problem")
    end
    
   end

 end

