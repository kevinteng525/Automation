class ApplicationController < ActionController::Base
  # Prevent CSRF attacks by raising an exception.
  # For APIs, you may want to use :null_session instead.
  protect_from_forgery with: :exception
  before_filter :authorize
  
 
  
  private
  
  def authorize
    if(session[:user_id] == nil)
      redirect_to login_url + "?path=" + ERB::Util.url_encode(request.fullpath)
      return
    else
      u = User.get_force_user_by_id(session[:user_id])
      if u
        @current_user = u
        user_subscribed_projects = User.get_subscribed_projects(session[:user_id])
        all_force_projects = Project.get_all_force_projects
        @subscribed_projects = Array.new()
        @subscribed_projects.concat(user_subscribed_projects)
        if(session[:user_role] == 0)#administrator
          #get the list of projects that the user can see
          all_force_projects.delete_if{|p| user_subscribed_projects.collect{|project| project.id}.include?(p.id)};
          @subscribed_projects.concat(all_force_projects)
          #set an indicator to show is the current user the administrator
          @is_current_user_administrator = true
        end    
      else
        session[:user_id] = nil
        session[:user_role] = nil
        redirect_to login_url, :notice => request.fullpath
        return
      end
    end  
  end
end
