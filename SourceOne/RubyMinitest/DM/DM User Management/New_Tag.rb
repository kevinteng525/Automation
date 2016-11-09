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
    @dmweb.usermenu_page.clickAdministration
    @dmweb.Tag_Manger.clickTagManager
    @dmweb.Tag_Manger.clickDefaultTag("hjz1")
    @dmweb.Tag_Manger.clickDeleteButton
    @dmweb.btn_id_helper.get_Dlg_btnID("Confirmation","Yes")
    @dmweb.usermenu_page.clickAdministration
    @dmweb.Tag_Manger.clickTagManager
    if @dmweb.Tag_Manger.click__Default_Tag("hjz1").exist?
      puts "Delete Created tag Failed!"
      
    else  
      puts "Delete Created tag Success!"  
      
    end
    @dmweb.close
  end
  
  #Make the test case run by order. 
  def self.test_order
    :alpha
  end    
  
  
 
   #Test case #4: New a search but cancel.
  def test_001_webid_3298
    @dmweb.login_page.open($DMWebSite)
    @dmweb.login_page.login_as($username, $password)   
    @dmweb.usermenu_page.clickAdministration
    @dmweb.Tag_Manger.clickTagManager
    @dmweb.Tag_Manger.clickNewButton
    @dmweb.btn_id_helper.get_Dlg_btnID_ForId("New Tag","dijit_form_ValidationTextBox_0","hjz1")
    sleep(3)
    @dmweb.btn_id_helper.enterAssignToMatter
    @dmweb.usermenu_page.clickAdministration
    @dmweb.Tag_Manger.clickTagManager
    if @dmweb.Tag_Manger.click__Default_Tag("hjz1").exist?
      puts "New Tag success!"
      assert(true,message="New Tag success!")
    else  
      puts "New Tag failed!"  
      assert(false,message="New Tag failed!")
    end 
    #@dmweb.btn_id_helper.get_Dlg_btnID("New Tag","OK")
    #@dmweb.btn_id_helper.get_Dlg_btnID_debug("Edit Tag","OK","dijit_form_Button_7")
    #@dmweb.usermenu_page.clickAdministration
 end

end