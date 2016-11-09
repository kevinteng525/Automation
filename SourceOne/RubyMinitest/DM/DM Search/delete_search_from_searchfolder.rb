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
require_relative '..\Objects\Matter_tree_items.rb'
require_relative '..\Objects\New_Matter_Dialog.rb'
require_relative '..\Objects\Matter_Properties_Dialog.rb'
gem "minitest"
require 'minitest/autorun'
require_relative '..\Objects\minitest_saber_base.rb'
require_relative '..\Objects\saber_base.rb'

#Smoke test cases.
class TestSuite < SaberTestBase
  include SaberBase
  
  def setup
    
    @dmweb = Site.new(Watir::Browser.new)
  end
  
  def teardown
    @dmweb1.close
    @dmweb2 = Site.new(Watir::Browser.new)
    @dmweb2.login_page.open($DMWebSite)
    @dmweb2.login_page.login_as($username, $password)
    @dmweb2.main_head_page.clickMatterLink
    @dmweb2.Mattertree_items.delete_matter_contextmenu($mattername,0)
    @dmweb2.btn_id_helper.get_Dlg_btnID("Confirmation","OK")
    @dmweb2.close
  end
  
  #Make the test case run by order. 
  def self.test_order
    :alpha
  end 
  
def test_delete_search_from_searchfolder_webid_1739
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    @dmweb.main_head_page.clickMatterLink
    @dmweb.main_matter_page.new_matter_click
    @dmweb.NewMatter_Dialog.set_MatterName($mattername)
    @dmweb.NewMatter_Dialog.click_newmatter_OK_bt
    @dmweb.main_head_page.clickMatterLink
    @dmweb.matter_tree_page.expandMatterNode
    @dmweb.matter_tree_page.clickLastMatterReview
    @dmweb.main_matter_page.new_search_click
    @dmweb.new_search_page.set_SearchName($searchname)
    @dmweb.new_search_page.searchclick_OK
    @dmweb.matter_tree_page.clickLastMatterReview
    @dmweb.main_matter_page.new_Folder_click
    @dmweb.new_search_page.set_FolderName($foldername)
    @dmweb.new_search_page.folderclick_OK
    @dmweb.matter_tree_page.keyboardSearchToFolder
    @dmweb.btn_id_helper.get_Dlg_btnID("Move Search",$foldername)
    @dmweb.btn_id_helper.get_Dlg_btnID("Move Search","OK")
    @dmweb.close
    @dmweb1 = Site.new(Watir::Browser.new)
    @dmweb1.login_page.open($DMWebSite)
    @dmweb1.login_page.login_as($username, $password)
    @dmweb1.main_head_page.clickMatterLink
    @dmweb1.Mattertree_items.matter_plus($mattername,0).click
    @dmweb1.Mattertree_items.matterreview_plus($mattername,0).click
    @dmweb1.Mattertree_items.folder_plus_matterreview($mattername,0,$foldername).click
    @dmweb1.Mattertree_items.search_in_folder($mattername,0,$foldername,$searchname).click
    @dmweb1.main_matter_page.delete_search_click
    @dmweb1.btn_id_helper.get_Dlg_btnID("Confirmation","OK")
    @dmweb1.main_head_page.clickMatterLink
    @dmweb1.Mattertree_items.matter_plus($mattername,0).click
    @dmweb1.Mattertree_items.matterreview_plus($mattername,0).click
    if !@dmweb1.Mattertree_items.verify_search_infolder_matterriview($mattername,0,$foldername,$searchname)
      puts "delete search from search folder success!"
      assert(true,message="delete search from search folder success!")
      
    else
      puts "delete search from search folder failed!"
      assert(false,message="delete search from search folder failed!")
    end
 end
 
end  