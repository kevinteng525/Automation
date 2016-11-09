class ProjectsController < ApplicationController
  before_action :set_project, only: [:show, :edit, :update, :destroy]

  # GET /projects
  # GET /projects.json
  def index
    @projects = Project.get_all_force_projects
  end

  # GET /projects/1
  # GET /projects/1.json
  def show
  end

  # GET /projects/new
  def new
    @project = Project.new
  end

  # GET /projects/1/edit
  def edit
    def @project.new_record?()
        false
    end
  end

  # POST /projects
  # POST /projects.json
  def create
    respond_to do |format|
      if Project.create_force_project(project_params)
        format.html { redirect_to projects_path, notice: 'Project was successfully created.' }
        format.json { render action: 'show', status: :created, location: @project }
      else
        format.html { render action: 'new' }
        format.json { render json: @project.errors, status: :unprocessable_entity }
      end
    end
  end

  # PATCH/PUT /projects/1
  # PATCH/PUT /projects/1.json
  def update
    respond_to do |format|
      if Project.update_force_project(params[:id], project_params)
        format.html { redirect_to @project, notice: 'Project was successfully updated.' }
        format.json { head :no_content }
      else
        format.html { render action: 'edit' }
        format.json { render json: @project.errors, status: :unprocessable_entity }
      end
    end
  end

  # DELETE /projects/1
  # DELETE /projects/1.json
  def destroy
    Project.delete_force_project(params[:id])
    respond_to do |format|
      format.html { redirect_to projects_url }
      format.json { head :no_content }
    end
  end
  
  def test_result_of_latest_task
    @result = Project.test_result_of_latest_task(params[:id])
  end
  
  def test_result_passrate_trend
    @result_trend = Project.test_result_passrate_trend(params[:id])
  end    
  
  private
    # Use callbacks to share common setup or constraints between actions.
    def set_project
      @project = Project.get_force_project_by_id(params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def project_params
      params.require(:project).permit(:name, :description, :vcs_server , :vcs_server_type, :vcs_server_root_path,:vcs_server_username,:vcs_server_password, :run_time)
    end
end
