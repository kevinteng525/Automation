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
  
  
 
   #Test case #4: Move_Search_toFolder_MatterReview_1744 not select folder to move
  def test_Move_Search_toFolder_MatterReview_webid_3299
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    @dmweb.main_matter_page.new_matter_click
    @dmweb.new_matter_page.set_MatterName($mattername)
    @dmweb.new_matter_page.click_OK
    sleep(5)
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
   # @dmweb.btn_id_helper.get_Dlg_btnID("Move Search",$foldername)
    @dmweb.btn_id_helper.get_Dlg_btnID("Move Search","OK")
    sleep(2)
    @dmweb.main_head_page.clickMatterLink
    @dmweb.matter_tree_page.expandMatterNode
    @dmweb.matter_tree_page.clickLastMatterReview
    if @dmweb.search_summary_page.validSearchFolder('Matter Review')
      puts "move the Search to Matter Review success!"
      assert(true,message="move the Search to Matter Review success!")
    else  
      puts "move the Search to Matter Review failed!"  
      assert(false,message="move the Search to Matter Review failed!")
    end 
   end
 end

