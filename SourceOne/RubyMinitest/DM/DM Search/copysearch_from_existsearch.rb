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
    #delete the matter created
    @dmweb.matter_tree_page.clickMatterClosed
    @dmweb.main_matter_page.delete_matter_click
    #@dmweb.new_search_page.click_OK
    @dmweb.Delete_Matter_page.click_delete_Matter_Ok
    #verify the matter is deleted.
    if @dmweb.matter_tree_page.verifyMatterInTree($mattername)
      puts "Delete matter failed!"
    else  
      puts "Delete matter success!" 
    end
    @dmweb.close
  end
  
  #Make the test case run by order. 
  def self.test_order
    :alpha
  end    
  
  
 
   #Test case #4:copy search from exist search
  def test_001_webid_1748
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    @dmweb.main_matter_page.new_matter_click
    @dmweb.new_matter_page.set_MatterName($mattername)
    @dmweb.new_matter_page.click_OK
    sleep(5)
    @dmweb.matter_tree_page.expandMatterNode
    @dmweb.matter_tree_page.clickLastCollectionarea
    @dmweb.main_matter_page.new_search_click
    @dmweb.new_search_page.set_SearchName($searchname)
    @dmweb.new_search_page.searchclick_OK
    sleep(2)
    @dmweb.new_search_page.tab_to_searchmenu
    @dmweb.btn_id_helper.get_Dlg_btnID("Copy Search","OK")
    #verify the search is created.
    @dmweb.main_head_page.clickMatterLink
    @dmweb.matter_tree_page.expandMatterNode
    @dmweb.matter_tree_page.clickLastCollectionarea
    if @dmweb.search_summary_page.validSearchNameGrid($searchname)&&@dmweb.search_summary_page.validSearchNameGrid($searchname+' - Copy')
      puts "copy search from_existsearch success!"
      assert(true,message="copy search from_existsearch success")
    else  
      puts "copy search from_existsearch failed!"  
      assert(false,message="copy search from_existsearch failed!")
    end 
    
   end

 end

