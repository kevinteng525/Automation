require 'watir-webdriver'
require 'watir-webdriver-performance'
require_relative '..\..\Variables\SUP_Variables_Configure.rb'
require_relative '..\..\Objects\Supervisor_Objects.rb'
require_relative '..\..\Objects\SUP_Login_Page.rb'
require_relative '..\..\Objects\SUP_Header_page.rb'
require_relative '..\..\Objects\SUP_Filter_Messages_Page.rb'
require_relative '..\..\Objects\SUP_MessageList_Page.rb'
require_relative '..\..\Objects\SUP_Report_Page.rb'
require_relative '..\..\Common\Data_Read.rb'
require_relative '..\..\Common\Date_Verify.rb'

gem "minitest"
require 'minitest/autorun'
#require_relative '..\Objects\minitest_saber_base.rb'

#Smoke test cases.
class TestSuite < Minitest::Test  
 
 def setup
   #get the data from sheet #1 of login user
   @excel_file1 = ReadExcel.new($excel_dir,1)
   #get the data from sheet #4 of mark
   @excel_file4 = ReadExcel.new($excel_dir,4)
   @data_row1 = @excel_file1.getRow 
   @data_row4 = @excel_file4.getRow
   @supervisorweb = SUPSite.new(Watir::Browser.new)
 end
 
 def teardown
   @excel_file1.closeExcel
   @excel_file4.closeExcel
   @supervisorweb.close
 end
 
  #Make the test case run by order.
  def self.test_order
    :alpha
  end

  def test_report_can_see_3_menu_items_Group_Member_Reviewer_Activity_Report_webid_4792
    sup_reviewer_report_testcase
  end

  def test_report_can_navigate_to_Group_Activity_Report_wizard_dialog_webid_4793
    sup_reviewer_report_dialog_testcase(1)
  end
  
  def test_report_can_navigate_to_Member_Activity_Report_wizard_dialog_webid_4794
    sup_reviewer_report_dialog_testcase(2)
  end  

  def test_report_can_navigate_to_Reviewer_Activity_Report_wizard_dialog_webid_4795
    sup_reviewer_report_dialog_testcase(3)
  end 

  def test_report_manager_can_see_reports_link_and_report_icon_webid_4796
    sup_verify_report_icon_and_link
  end
  
  def test_report_check_8_tabs_webid_4797
    sup_report_check_8_tabs
  end

  def test_report_navigate_to_configuration_report_wizard_page_webid_4798
    sup_manager_report_testcase(1)
  end

  def test_report_navigate_to_message_detail_report_wizard_page_webid_4799
    sup_manager_report_testcase(2)
  end

  def test_report_navigate_to_business_policy_activity_report_wizard_page_webid_4800
    sup_manager_report_testcase(3)
  end

  def test_report_navigate_to_group_activity_report_wizard_page_webid_4801
    sup_manager_report_testcase(4)
  end
  
  def test_report_navigate_to_member_activity_report_wizard_page_webid_4802
    sup_manager_report_testcase(5)
  end
  
  def test_report_navigate_to_reviewer_activity_report_wizard_page_webid_4803
    sup_manager_report_testcase(6)
  end
  
  def test_report_navigate_to_query_activity_report_wizard_page_webid_4804
    sup_manager_report_testcase(7)
  end  

  def test_report_navigate_to_license_availability_wizard_page_webid_4805
    sup_manager_report_testcase(8)
  end 
  
#===============================================================================================================================================

  #functions for test case
  #login supervisor web.
  def sup_login
    @input_data1 = @excel_file1.getUserData(@data_row1,1)
    @supervisorweb.SUPlogin_page.login_function(@input_data1)
    @supervisorweb.SUPfilter_message_page.close_filter_dialog
    @supervisorweb.load_time    
  end
  
  #navigate to manager report page.
  def sup_navigate_to_report_tab
    sup_login
    @supervisorweb.SUPheader_page.clickReportTab
    @supervisorweb.load_time
  end 

  #test 8 tabs count.
  def sup_report_check_8_tabs
    sup_navigate_to_report_tab
    result = @supervisorweb.SUPReport_Page.verifyReportTabList
    if result == true
      assert(true, "The 8 report tab is correct.")
    else  
      assert(false, "The 8 report tab  is incorrect.")
    end    
  end  

  #test 8 tabs.
  def sup_manager_report_testcase(itab)
    sup_navigate_to_report_tab
    #verify every tab.
    @supervisorweb.SUPReport_Page.clickReportTabRow(itab)
    wizard_result = @supervisorweb.SUPReport_Page.verifyReportPageWizard
    if wizard_result == true
      assert(true, "The report tab "+itab.to_s+" wizard is correct.")
    else  
      assert(false, "The report tab "+itab.to_s+" wizard is incorrect.")
    end
  end 
  
  #test report tab link and report icon can be found
  def sup_verify_report_icon_and_link
    sup_login
    if @supervisorweb.SUPheader_page.getReportIcon == true
      if @supervisorweb.SUPheader_page.getReportTabLink == true
        assert(true, "The report tab link and report icon can be found.")
      else
        assert(false, "The report tab link cannot be found.")
      end
    else
      assert(false, "The report icon cannot be found.")  
    end
  end

  #reviewer report test cases.
  def sup_reviewer_report_testcase
    sup_login
    @supervisorweb.SUPheader_page.clickReportIcon
    link_result = @supervisorweb.SUPReport_Page.verifyReportIconLink
    if link_result == true
      assert(true, "The report links name are correct.")
    else  
      assert(false, "The report links name are incorrect.")
    end    
  end
   
  def sup_reviewer_report_dialog_testcase(irow)
    sup_login
    @supervisorweb.SUPheader_page.clickReportIcon
    arr_report_icon_links = ["Group Activity Report", "Member Activity Report", "Reviewer Activity Report"]
    @supervisorweb.SUPReport_Page.clickReportIconRow(irow)
    result = @supervisorweb.SUPReport_Page.verifyReportDialogTitle(arr_report_icon_links[irow-1]) 
    if result == true
      assert(true, "The report link name "+arr_report_icon_links[irow-1]+" is correct.")
    else  
      assert(false, "The report link name "+arr_report_icon_links[irow-1]+" is incorrect.")
    end    
  end   
   
end
 
