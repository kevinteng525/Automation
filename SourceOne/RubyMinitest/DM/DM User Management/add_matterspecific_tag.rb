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
    @dmweb1.Mattertree_items.delete_matter_contextmenu("hjz",0)
    @dmweb1.btn_id_helper.get_Dlg_btnID("Confirmation","OK")
    sleep(2)
    @dmweb1.main_head_page.clickMatterLink
    @dmweb1.Mattertree_items.delete_matter_contextmenu("jxj",0)
    @dmweb1.btn_id_helper.get_Dlg_btnID("Confirmation","OK")
    sleep(2)
    @dmweb1.close
  end
  
  #Make the test case run by order. 
  def self.test_order
    :alpha
  end 
  
def test_add_matterspecific_tag_webid_675
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)
    @dmweb.main_head_page.clickMatterLink
    @dmweb.main_matter_page.new_matter_click
    @dmweb.new_matter_page.set_MatterName("hjz")
    @dmweb.new_matter_page.click_OK
    @dmweb.main_head_page.clickMatterLink
    @dmweb.main_matter_page.new_matter_click
    @dmweb.new_matter_page.set_MatterName("jxj")
    @dmweb.new_matter_page.click_OK
    @dmweb.main_head_page.clickMatterLink
    @dmweb.Mattertree_items.matter_click("jxj",0).click
    @dmweb.main_matter_page.matter_properties_click
    sleep(10)
    @dmweb.btn_id_helper.get_Dlg_btnID("jxj - Matter Properties","Tags")
    @dmweb.Matter_Properties_Dialog.new_tag("jxj - Matter Properties","hejz","111","SaddleBrown ")
    @dmweb.btn_id_helper.get_Dlg_btnID("jxj - Matter Properties","OK")
    @dmweb.main_head_page.clickMatterLink
    @dmweb.Mattertree_items.matter_click("jxj",0).click
    @dmweb.main_matter_page.matter_properties_click
    sleep(2)
    @dmweb.btn_id_helper.get_Dlg_btnID("jxj - Matter Properties","Tags")
    if @dmweb.Matter_Properties_Dialog.findtag("hejz").exist?
      puts "add tag success!"
    else
      puts "add tag failed!" 
    end
    @dmweb.btn_id_helper.get_Dlg_btnID("jxj - Matter Properties","OK") 
    @dmweb.main_head_page.clickMatterLink
    @dmweb.Mattertree_items.matter_click("hjz",0).click
    @dmweb.main_matter_page.matter_properties_click
    sleep(2)
    @dmweb.btn_id_helper.get_Dlg_btnID("hjz - Matter Properties","Tags")
    if !@dmweb.Matter_Properties_Dialog.findtag("hejz").exist?
      puts "other matter can't find matter specific tag success!"
      assert(true,message="other matter can't find matter specific tag success!")    
    else
      puts "other matter can't find matter specific tag failed!" 
      assert(false,message="other matter can't find matter specific tag failed!")      
    end 
   
 end
end  