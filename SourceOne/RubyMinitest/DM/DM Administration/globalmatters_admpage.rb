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
    @dmweb.close
    @dmweb1 = Site.new(Watir::Browser.new)
    @dmweb1.login_page.open($DMWebSite)
    @dmweb1.login_page.login_as($username, $password)
    @dmweb1.main_head_page.clickMatterLink
    @dmweb1.Mattertree_items.delete_matter_contextmenu($mattername+10000.to_s,0)
    @dmweb1.btn_id_helper.get_Dlg_btnID("Confirmation","OK")
    @dmweb1.close
  end
  
  #Make the test case run by order. 
  def self.test_order
    :alpha
  end    
  
  
  #Test case #1: create matter in all matters.
  def test_create_matter_allmatters_webid_1581   
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    @dmweb.Mattertree_items.all_matters_click.click
    @dmweb.main_matter_page.new_matter_click
    @dmweb.NewMatter_Dialog.set_MatterName($mattername+10000.to_s)
    @dmweb.NewMatter_Dialog.click_newmatter_OK_bt
    @dmweb.usermenu_page.clickAdministration
    @dmweb.main_administration_page.allmatters_bt.click
    sleep(2)
    if @dmweb.main_administration_page.findmatter_in_allmatters($mattername+10000.to_s).exist?
      puts "can see matter in all matters of administration page success!"
      assert(true,message="can see matter in all matters of administration page  success!")
    else  
      puts "can see matter in all matters of administration page  failed!"  
      assert(false,message="can see matter in all matters of administration page  failed!")
    end 
  end
 end