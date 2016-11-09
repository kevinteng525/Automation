require 'watir-webdriver'
require_relative '..\Variables\SUP_Variables_Configure.rb'
require_relative '..\Objects\Supervisor_Objects.rb'
gem "minitest"
require "minitest/autorun"

#Header page==============================================================
class SUPHeaderPage < BrowserContainer

  #verify the user name equals to the login name.
  def verify_login_name(loginname)
    if username_text.exists? then
      len_name = username_text.text.length
      head_name = username_text.text[0,len_name-1]
      if head_name == loginname then
        return true
      else
        print head_name
        return false
      end
    else
      puts "error: it cannot find the user name header."
      return false
    end
  end

  #click on the ¨‹ button.
  def clickArrow
    if header_Menu_Arrow.exists?
      header_Menu_Arrow.click
      @browser.wait($TIMEOUT)
    else
      puts "error: it cannot find the arrow button."
      return false
    end
  end
  
  #click on the log out link.
  def clickLogout
    if log_out_link.exists?
      log_out_link.click
      @browser.wait($TIMEOUT)
    else
      clickArrow
      log_out_link.click
      @browser.wait($TIMEOUT)
    end
  end  

  #click on the Filter icon.
  def clickFilters
    if filters_icon.exists?
      filters_icon.click
      @browser.wait($TIMEOUT)
    else
      puts "error: Filters icon cannot be found in the header bar."
    end
  end

  #get report tab.
  def getReportTabLink
    if reports_tab.exists?
      return true
    else
      puts "Function 'getReportTabLink' of SUP_Header_Page.rb - error: Report tab cannot be found in the header bar."
      return false
    end
  end

  #click on the report tab.
  def clickReportTab
    if reports_tab.exists?
      reports_tab.click
      @browser.wait($TIMEOUT)
    else
      puts "Function 'clickReportTab' of SUP_Header_Page.rb - error: Report tab cannot be found in the header bar."
    end
  end
  
  #click on report icon.
  def clickReportIcon
    if report_icon.exists?
      report_icon.click
      @browser.wait($TIMEOUT)
    else
      puts "Function 'clickReportIcon' of SUP_Header_Page.rb - error: Report icon cannot be found in the tool bar."
    end
  end

  #get report icon.
  def getReportIcon
    if report_icon.exists?
      return true
    else
      puts "Function 'getReportIcon' of SUP_Header_Page.rb - error: Report icon cannot be found in the tool bar."
      return false
    end
  end

  private

  #The user name bar.
  def username_text
    @browser.div(:class => "menu-item menu-item-username")
  end

  #header logo for Supervisor
  def head_logo_image
    @browser.img(:id => "header_logo")
  end

  #Reviewer tab.
  def reviewer_tab
    @browser.a(:id => "header_link_reviewer")
  end

  #Reports tab.
  def reports_tab
    @browser.a(:id => "header_link_report")
  end

  #the ¨‹ button.
  def header_Menu_Arrow
    @browser.span(:class => "headerMenuArrow")
  end

  #Log out link.
  def log_out_link
    @browser.a(:id => "lnkLogout")
  end

  #Filters icon.
  def filters_icon
    @browser.span(:class => "dijitReset dijitInline dijitIcon toolbar_icon toolbar_icon_filter")
  end

  #Refresh icon.
  def refresh_icon
    @browser.span(:class => "dijitReset dijitInline dijitIcon toolbar_icon toolbar_icon_refresh")
  end
  
  #Report icon.
  def report_icon
    @browser.span(:class => "dijitReset dijitInline dijitIcon toolbar_icon toolbar_icon_report")
  end
end 


