class BuildsController < ApplicationController
  before_action :set_build, only: [:show, :edit, :update, :destroy]

  # GET /builds
  # GET /builds.json
  def index
    @builds = Build.get_all_force_builds
  end

  # GET /builds/1
  # GET /builds/1.json
  def show
  end

  # GET /builds/new
  def new
    @build = Build.new
  end

  # GET /builds/1/edit
  def edit
    def @build.new_record?()
        false
    end
  end

  # POST /builds
  # POST /builds.json
  def create
    build = Build.create_force_build(build_params)
    respond_to do |format|
      if build
        format.html { redirect_to build_path(build), notice: 'Build was successfully created.' }
        format.json { render action: 'show', status: :created, location: @build }
      else
        format.html { render action: 'new' }
        format.json { render json: @build.errors, status: :unprocessable_entity }
      end
    end
  end

  # PATCH/PUT /builds/1
  # PATCH/PUT /builds/1.json
  def update
    respond_to do |format|
      if Build.update_force_build(params[:id],build_params)
        format.html { redirect_to @build, notice: 'Build was successfully updated.' }
        format.json { head :no_content }
      else
        format.html { render action: 'edit' }
        format.json { render json: @build.errors, status: :unprocessable_entity }
      end
    end
  end

  # DELETE /builds/1
  # DELETE /builds/1.json
  def destroy
    Build.delete_force_build(params[:id])
    respond_to do |format|
      format.html { redirect_to builds_url }
      format.json { head :no_content }
    end
  end

  private
    # Use callbacks to share common setup or constraints between actions.
    def set_build
      @build = Build.get_force_build_by_id(params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def build_params
      params.require(:build).permit(:product_id, :name, :build_type, :status, :branch, :number, :location, :description)
    end
end
