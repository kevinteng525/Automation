require 'watir-webdriver'
require 'watir-webdriver-performance'
require_relative '..\Variables\SUP_Variables_Configure.rb'
require_relative '..\Objects\Supervisor_Objects.rb'
require_relative '..\Common\Data_Read.rb'
gem "minitest"
require "minitest/autorun"

#Login page==============================================================
class SUPReportPage < BrowserContainer
  
  #get the count of report tab list.
  def getReportTabListCount
    if report_tab_list.exists?
      return report_tab_list.attribute_value("childElementCount").to_i
    else
      puts "Function 'getReportListCount' of SUP_MessageList_Page.rb - error: report tab list cannot be found in the report page."
    end
  end
  
  #get the report tab name.
  def getReportTabName(tab_num)
    if report_tab(tab_num.to_i-1).exists?
      return report_tab(tab_num.to_i-1).attribute_value("textContent").to_s
    else
      puts "Function 'getReportTabName' of SUP_MessageList_Page.rb - error: report tab #"+tab_num.to_s+"  cannot be found in the report page."
    end
  end
 
  #click on tab.
  def clickReportTabRow(tab_num)
    if report_tab(tab_num.to_i-1).exists?
      report_tab(tab_num.to_i-1).click
      @browser.wait($TIMEOUT)
    else
      puts "Function 'clickReportTab' of SUP_MessageList_Page.rb - error: report tab #"+tab_num.to_s+"  cannot be found in the report page."
    end
  end
 
  #verify it should has 8 tabs in report page and its content.
  def verifyReportTabList
    count = getReportTabListCount
    result = "true"
    if count == 8
      arr_tab_name = ["Configuration","Message Detail","Business Policy Activity","Group Activity","Member Activity","Reviewer Activity","Query Activity","License Availability"]
      for i in 1..8
        if getReportTabName(i).include?arr_tab_name[i-1]
          result +="true"
        else
          puts "Function 'verifyReportTabList' of SUP_MessageList_Page.rb - error: report tab #"+arr_tab_name[i-1].to_s+"  cannot be found in the report page."
          result +="false"
        end
      end
    else
      puts "Function 'verifyReportTabList' of SUP_MessageList_Page.rb - error: it only has "+count.to_s+" in the report tab."
      result +="false"
    end
    
    if result.include?"false"
      return false
    else
      return true
    end
    
  end

  #verify report page wizard content.
  def verifyReportPageWizard
    if report_date_from_textbox.exists?
      return true
    else
      puts "Function 'verifyReportPageWizard' of SUP_MessageList_Page.rb - error: date from text box cannot be found."
      return false
    end
    if report_date_to_textbox.exists?
      return true
    else
      puts "Function 'verifyReportPageWizard' of SUP_MessageList_Page.rb - error: date to text box cannot be found."
      return false
    end    
  end

  #click on reviewer report icon for every report link.
  def clickReportIconRow(report_row)
    if reviewer_report_row(report_row.to_i-1).exists?
      reviewer_report_row(report_row.to_i-1).click
    else
      puts "Function 'clickReportIconRow' of SUP_MessageList_Page.rb - error: reviewer report icon row cannot be found."
    end
  end
 
  #get the reviewer report icon row content.
  def getReportIconRowContent(report_row)
    if reviewer_report_row(report_row.to_i-1).exists?
      return reviewer_report_row(report_row.to_i-1).attribute_value("aria-label").to_s
    else
      puts "Function 'getReportIconRowContent' of SUP_MessageList_Page.rb - error: reviewer report icon row cannot be found."
    end      
  end    
  
  #verify 3 report link can be found.
  def verifyReportIconLink
    result = "true"
    arr_report_icon_links = ["Group Activity Report", "Member Activity Report", "Reviewer Activity Report"]
    for i in 1..3
      if getReportIconRowContent(i).include?arr_report_icon_links[i-1]
        result += "true"
      else
        puts "Function 'verifyReportIconLink' of SUP_MessageList_Page.rb - error: reviewer report icon link cannot be found."
        result += "false"
      end
    end   
    if result.include?"false"
      return false
    else
      return true
    end
  end
  
  #verify the Reviewer Report dialog box pop up and title is correct.
  def verifyReportDialogTitle(title_name)
    if reviewer_report_dialog_title.exists?
      if reviewer_report_dialog_title.attribute_value("textContent").include?title_name
        return true
      else
        puts "Function 'verifyReportDialogTitle' of SUP_MessageList_Page.rb - error: reviewer report "+title_name+" dialog box cannot be found."
        return false
      end
    else
      puts "Function 'verifyReportDialogTitle' of SUP_MessageList_Page.rb - error: reviewer report dialog box cannot be found."
      return false
    end
  end
  
  
  private

  #report tab list table.
  def report_tab_list
    @browser.div(:id =>"ReportMain_tablist")
  end
 
  #report tab.
  def report_tab(tab_num)
    @browser.div(:id =>"ReportMain_tablist").div(:class => /dijitTabInner dijitTabContent dijitTab/, :index => tab_num)
  end
 
  #date from.
  def report_date_from_textbox
    @browser.div(:class =>"reportFieldSet dijitContentPane").div(:id =>"widget_dijit_form_DateTextBox_0")
  end
  
  #date to.
  def report_date_to_textbox
    @browser.div(:class =>"reportFieldSet dijitContentPane").div(:id =>"widget_dijit_form_DateTextBox_1")
  end
  
  #=====================================Reviewer Report===============================================
  #Reviewer Report row
  def reviewer_report_row(report_row)
    @browser.table(:id =>"dijit_DropDownMenu_3").tbody(:class =>"dijitReset").tr(:class =>"dijitReset dijitMenuItem", :index => report_row)
  end

  #Reviewer Report dialog box title.
  def reviewer_report_dialog_title
    @browser.span(:class =>"dijitDialogTitle")
  end
   
end

